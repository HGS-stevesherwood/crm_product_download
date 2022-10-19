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

        public ActionResult Publish(int itemId)
        {
            var item = ItemManager.Instance.GetItem(itemId, ModuleContext.ModuleId);
            item.ItemPublished = "true";
            ItemManager.Instance.UpdateItem(item);
            return RedirectToDefaultRoute();
        }

        public ActionResult UnPublish(int itemId)
        {
            var item = ItemManager.Instance.GetItem(itemId, ModuleContext.ModuleId);
            item.ItemPublished = "false";
            ItemManager.Instance.UpdateItem(item);
            return RedirectToDefaultRoute();
        }

        public ActionResult Promote(int itemId)
        {
            var item = ItemManager.Instance.GetItem(itemId, ModuleContext.ModuleId);
            item.ItemLatest = "true";
            ItemManager.Instance.UpdateItem(item);
            return RedirectToDefaultRoute();
        }

        public ActionResult Demote(int itemId)
        {
            var item = ItemManager.Instance.GetItem(itemId, ModuleContext.ModuleId);
            item.ItemLatest = "false";
            ItemManager.Instance.UpdateItem(item);
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

        public ActionResult Release(int itemId)
        {

            var item = ItemManager.Instance.GetItem(itemId, ModuleContext.ModuleId);

            GetSignedReleaseURL(item);

            item.ItemAvailable = item.ItemDuration;

            ItemManager.Instance.UpdateItem(item);

            return Redirect(item.ItemSignedUrl);
        }


        public ActionResult Installation(int itemId)
        {

            var item = ItemManager.Instance.GetItem(itemId, ModuleContext.ModuleId);

            GetSignedInstallationURL(item);

            item.ItemAvailable = item.ItemDuration;

            ItemManager.Instance.UpdateItem(item);

            return Redirect(item.ItemSignedUrl);
        }

        public void GetSignedURL(Item item)
        {
            //Create object of FileInfo for specified path           
            FileInfo pkfile = new FileInfo(Server.MapPath("~/private.pem"));
            
            item.ItemSignedUrl = AmazonCloudFrontUrlSigner.GetCannedSignedURL(
            AmazonCloudFrontUrlSigner.Protocol.https,
            item.ItemUrl,
            pkfile,
            item.ItemPath,
            item.ItemPrivateKeyId,
            item.ItemDuration);
        }

        public void GetSignedReleaseURL(Item item)
        {
            //Create object of FileInfo for specified path           
            FileInfo pkfile = new FileInfo(Server.MapPath("~/private.pem"));

            item.ItemSignedUrl = AmazonCloudFrontUrlSigner.GetCannedSignedURL(
            AmazonCloudFrontUrlSigner.Protocol.https,
            item.ItemUrl,
            pkfile,
            item.ItemReleasePath,
            item.ItemPrivateKeyId,
            item.ItemDuration);
        }


        public void GetSignedInstallationURL(Item item)
        {
            //Create object of FileInfo for specified path           
            FileInfo pkfile = new FileInfo(Server.MapPath("~/private.pem"));

            item.ItemSignedUrl = AmazonCloudFrontUrlSigner.GetCannedSignedURL(
            AmazonCloudFrontUrlSigner.Protocol.https,
            item.ItemUrl,
            pkfile,
            item.ItemInstallationPath,
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
                item.ItemPublished = item.ItemPublished;
                item.ItemLatest = item.ItemLatest;
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
                existingItem.ItemPublished = item.ItemPublished;
                existingItem.ItemLatest = item.ItemLatest;
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
        public ActionResult Index(string sortOrder, string searchString, string viewOrder)
        {
            if (Request.IsAuthenticated)
            {

                //Setup View
                ViewBag.CategorySortParam = sortOrder == "category" ? "category_desc" : "category";
                ViewBag.NameSortParam = sortOrder == "name" ? "name_desc" : "name";
                ViewBag.VersionSortParam = sortOrder == "version" ? "version_desc" : "version";
                ViewBag.PlatformSortParam = sortOrder == "platform" ? "platform_desc" : "platform";
                ViewBag.LatestSortParam = viewOrder == "latest" ? "latest" : "latest";
                ViewBag.PreviousSortParam = viewOrder == "previous" ? "previous" : "previous";
                ViewBag.AllSortParam = viewOrder == "staged" ? "staged" : "staged";


                //Setup Filter
                ViewBag.CurrentFilter = searchString;


                //Setup ViewOrder
                ViewBag.ViewFilter = viewOrder;


                //Setup ViewItems
                var items = ItemManager.Instance.GetItems(ModuleContext.ModuleId);


                //Handle Request Events
                if (!String.IsNullOrEmpty(ViewBag.ViewFilter))
                {
                    switch (viewOrder)
                    {
                        case "latest":
                            Func<Item, bool> isLatest = i => i.ItemLatest == "true";
                            var latestItems = items.Where(isLatest);
                            Func<Item, bool> isPublishedLatest = i => i.ItemPublished == "true";
                            var publishedLatestItems = latestItems.Where(isPublishedLatest);
                            items = publishedLatestItems;
                            break;
                        case "previous":
                            Func<Item, bool> isPrevious = i => i.ItemLatest == "false";
                            var previousItems = items.Where(isPrevious);
                            Func<Item, bool> isPublishedPrevious = i => i.ItemPublished == "true";
                            var publishedPreviousItems = previousItems.Where(isPublishedPrevious);
                            items = publishedPreviousItems;
                            break;
                        case "staged":
                            Func<Item, bool> isStaged = i => i.ItemPublished == "false";
                            var stagedItems = items.Where(isStaged);
                            items = stagedItems;
                            break;
                        default:
                            Func<Item, bool> isDefault = i => i.ItemLatest == "true";
                            var defaultItems = items.Where(isDefault);
                            Func<Item, bool> isPublishedDefault = i => i.ItemPublished == "true";
                            var defaultLatestItems = defaultItems.Where(isPublishedDefault);
                            items = defaultLatestItems;
                            break;
                    }

                }
                else
                {
                    Func<Item, bool> isDefault = i => i.ItemLatest == "true";
                    var defaultItems = items.Where(isDefault);
                    Func<Item, bool> isPublishedDefault = i => i.ItemPublished == "true";
                    var defaultLatestItems = defaultItems.Where(isPublishedDefault);
                    items = defaultLatestItems;
                }

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
                        items = items.OrderBy(i => i.ItemCategory);
                        break;
                }

                return View(items);

            } else
            {

                return Redirect("https://" + Request.Url.Host + Request.ApplicationPath + "/Company");

            }

        }

    }
}
