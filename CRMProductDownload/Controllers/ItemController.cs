/*
' Copyright (c) 2022 CRM.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Mvc;
using CRM.Modules.CRMProductDownload.Components;
using CRM.Modules.CRMProductDownload.Models;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using Amazon.Runtime;
using Amazon.S3;
using Amazon;
using System.Net.Http;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;
using System.Web.Configuration;
using Amazon.CloudFront;
using System.Reflection;

namespace CRM.Modules.CRMProductDownload.Controllers
{

    [DnnHandleError]
    public class ItemController : DnnController
    {

        public ActionResult Delete(int itemId)
        {
            ItemManager.Instance.DeleteItem(itemId, ModuleContext.ModuleId);
            return RedirectToDefaultRoute();
        }

        public ActionResult Activate(int itemId)
        {

            var item = ItemManager.Instance.GetItem(itemId, ModuleContext.ModuleId);

            GetSignedURL(item);

            item.ItemAvailable = item.ItemDuration;

            ItemManager.Instance.UpdateItem(item);

            return Redirect(item.ItemSignedUrl);
        }

        public void GetSignedURL(Item item)
        {
            //Create object of FileInfo for specified path           
            FileInfo pkfile = new FileInfo(Server.MapPath("~/App_Data/private.pem"));
            
            item.ItemSignedUrl = AmazonCloudFrontUrlSigner.GetCannedSignedURL(
            AmazonCloudFrontUrlSigner.Protocol.https,
            item.ItemUrl,
            pkfile,
            item.ItemPath,
            item.ItemPrivateKeyId,
            item.ItemDuration);
        }

        public ActionResult Edit(int itemId = -1)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);

            var userlist = UserController.GetUsers(PortalSettings.PortalId);
            var users = from user in userlist.Cast<UserInfo>().ToList()
                        select new SelectListItem { Text = user.DisplayName, Value = user.UserID.ToString() };

            ViewBag.Users = users;

            var item = (itemId == -1)
                 ? new Item { ModuleId = ModuleContext.ModuleId }
                 : ItemManager.Instance.GetItem(itemId, ModuleContext.ModuleId);

            return View(item);
        }

        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Edit(Item item)
        {
           
            if (ModelState.IsValid && item.ItemId == -1)
            {
                item.CreatedByUserId = User.UserID;
                item.CreatedOnDate = DateTime.UtcNow;
                item.LastModifiedByUserId = User.UserID;
                item.LastModifiedOnDate = DateTime.UtcNow;
                item.ItemCategory = item.ItemCategory;
                item.ItemName = item.ItemName;
                item.ItemPath = item.ItemPath;
                item.ItemExtension = item.ItemExtension;
                item.ItemPlatform = item.ItemPlatform;
                item.ItemVersion = item.ItemVersion;
                item.ItemUrl = item.ItemUrl;

                ItemManager.Instance.CreateItem(item);

                return RedirectToDefaultRoute();
            }

            if (ModelState.IsValid && item.ItemId != -1)
            {
                var existingItem = ItemManager.Instance.GetItem(item.ItemId, item.ModuleId);

                existingItem.LastModifiedByUserId = User.UserID;
                existingItem.LastModifiedOnDate = DateTime.UtcNow;
                existingItem.ItemCategory = item.ItemCategory;
                existingItem.ItemName = item.ItemName;
                existingItem.ItemPath = item.ItemPath;
                existingItem.ItemExtension = item.ItemExtension;
                existingItem.ItemPlatform = item.ItemPlatform;
                existingItem.ItemDescription = item.ItemDescription;
                existingItem.ItemVersion = item.ItemVersion;
                existingItem.ItemUrl = item.ItemUrl;

                existingItem.AssignedUserId = item.AssignedUserId;

                ItemManager.Instance.UpdateItem(existingItem);

                return RedirectToDefaultRoute();
            }

            return View(item);
            
        }

        [ModuleAction(ControlKey = "Edit", TitleKey = "AddItem")]
        public ActionResult Index(string sortOrder, string searchString)
        {


            ViewBag.CategorySortParam = sortOrder == "category" ? "category_desc" : "category";
            ViewBag.NameSortParam = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.VersionSortParam = sortOrder == "version" ? "version_desc" : "version";
            ViewBag.PlatformSortParam = sortOrder == "platform" ? "platform_desc" : "platform";
            
            ViewBag.CurrentFilter = searchString;

            var items = ItemManager.Instance.GetItems(ModuleContext.ModuleId);


            if (!String.IsNullOrEmpty(ViewBag.CurrentFilter))
            {
                items = items.Where(i => i.ItemName.Contains(ViewBag.CurrentFilter)
                                       || i.ItemVersion.Contains(ViewBag.CurrentFilter)
                                       || i.ItemCategory.Contains(ViewBag.CurrentFilter)
                                       || i.ItemPlatform.Contains(ViewBag.CurrentFilter));
            }

            switch (sortOrder)
            {
                case "category":
                    items = items.OrderBy(i => i.ItemCategory);
                    break;
                case "category_desc":
                    items = items.OrderByDescending(i => i.ItemCategory);
                    break;
                case "name":
                    items = items.OrderBy(i => i.ItemName);
                    break;
                case "name_desc":
                    items = items.OrderByDescending(i => i.ItemName);
                    break;
                case "version":
                    items = items.OrderBy(i => i.ItemVersion);
                    break;
                case "version_desc":
                    items = items.OrderByDescending(i => i.ItemVersion);
                    break;
                case "platform":
                    items = items.OrderBy(i => i.ItemPlatform);
                    break;
                case "platform_desc":
                    items = items.OrderByDescending(i => i.ItemPlatform);
                    break;
                default:
                    break;
            }

            return View(items);
        }

    }
}
