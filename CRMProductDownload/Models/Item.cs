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
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;

namespace CRM.Modules.CRMProductDownload.Models
{
    [TableName("CRMProductDownload_Items")]
    //setup the primary key for table
    [PrimaryKey("ItemId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Items", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    [Scope("ModuleId")]
    public class Item
    {
        ///<summary>
        /// The ID of your object with the name of the ItemName
        ///</summary>
        public int ItemId { get; set; } = -1;

        ///<summary>
        /// A string with the name of the ItemUrl
        ///</summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a valid URL i.e https://d11ys8qbt2p17g.cloudfront.net")]
        public string ItemUrl { get; set; } = "";

        ///<summary>
        /// A string with the name of the ItemName
        ///</summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a valid product name i.e envi563-linux")]
        public string ItemName { get; set; }

        ///<summary>
        /// A string with the path of the object
        ///</summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a valid path path/item i.e envi563/envi563-linux.tar.gz")]
        public string ItemPath { get; set; }

        ///<summary>
        /// A string with the extension of the object
        ///</summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a valid extension i.e gz, zip")]
        public string ItemExtension { get; set; }

        ///<summary>
        /// A string with the platform of the object
        ///</summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a valid platform i.e. Linux, Windows")]
        public string ItemPlatform { get; set; }

        ///<summary>
        /// A string with the version of the object
        ///</summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a valid version i.e. 5.6.3")]
        public string ItemVersion { get; set; }

        ///<summary>
        /// A string with the description of the object
        ///</summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a valid product description")]
        public string ItemDescription { get; set; }

        ///<summary>
        /// A string with the group of the object
        ///</summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a valid product category")]
        public string ItemCategory { get; set; }

        ///<summary>
        /// The date the object is available
        ///</summary>
        public DateTime ItemAvailable { get; set; } = DateTime.UtcNow;

        ///<summary>
        /// An integer with the user id of the assigned user for the object
        ///</summary>
        public int AssignedUserId { get; set; } = 1;

        ///<summary>
        /// The ModuleId of where the object was created and gets displayed
        ///</summary>
        public int ModuleId { get; set; }

        ///<summary>
        /// An integer for the user id of the user who created the object
        ///</summary>
        public int CreatedByUserId { get; set; } = -1;

        ///<summary>
        /// An integer for the user id of the user who last updated the object
        ///</summary>
        public int LastModifiedByUserId { get; set; } = -1;

        ///<summary>
        /// The date the object was created
        ///</summary>
        public DateTime CreatedOnDate { get; set; } = DateTime.UtcNow;

        ///<summary>
        /// The date the object was updated
        ///</summary>
        public DateTime LastModifiedOnDate { get; set; } = DateTime.UtcNow;

        ///<summary>
        /// The date the object expires
        ///</summary>
        public DateTime ItemDuration { get; set; } = DateTime.Now.AddDays(Double.Parse("25"));

        ///<summary>
        /// The state of the publised product
        ///</summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide 'true' or 'false' value")]
        public string ItemPublished { get; set; }

        ///<summary>
        /// The state of latest product
        ///</summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide 'true' or 'false' value")]
        public string ItemLatest { get; set; }

        ///<summary
        /// A string with the item private key id
        ///</summary>
        public string ItemPrivateKeyId { get; set; } = "K33HASPSL3ZKRV";

        ///<summary>
        /// A string with the item signed url
        ///</summary>
        public string ItemSignedUrl { get; set; } = "";

    }
}
