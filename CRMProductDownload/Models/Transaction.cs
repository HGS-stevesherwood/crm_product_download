using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;

namespace CRM.Modules.CRMProductDownload.Models
{
    [TableName("CRMProductDownload_Transactions")]
    //setup the primary key for table
    [PrimaryKey("TransactionId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Transactions", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    [Scope("ModuleId")]
    public class Transaction
    {

        ///<summary>
        /// The ID of your object with the id of the transaction
        ///product version os userid first name last name country ipaddress affilliate
        ///</summary>
        public int TransactionId { get; set; } = -1;

        ///<summary>
        /// The ID of your object with the name of the ItemName
        ///</summary>
        public int ItemId { get; set; } = -1;

        ///<summary>
        /// A string with the name of the ItemName
        ///</summary>
        public string ItemName { get; set; }

        ///<summary>
        /// A string with the version of the object
        ///</summary>
        public string ItemVersion { get; set; }

        ///<summary>
        /// A string with the platform of the object
        ///</summary>
        public string ItemPlatform { get; set; }

        ///<summary>
        /// The date the object transaction occured
        ///</summary>
        public DateTime TransactionDate { get; set; }

        ///<summary>
        /// A string with the username for the transaction
        ///</summary>
        public string Username { get; set; }

        ///<summary>
        /// A string with the affiliate for the transaction
        ///</summary>
        public int Affiliate { get; set; }

        ///<summary>
        /// A string with the user ip address
        ///</summary>
        public string IpAddress { get; set; }

        ///<summary>
        /// The ModuleId of where the object was created and gets displayed
        ///</summary>
        public int ModuleId { get; set; }
    }
}