using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Startapp.Shared.Models
{
    public class AppUser : IdentityUser, IAuditableEntity
    {
        public virtual string FullName
        {
            get
            {
                string fullName = null;
                if (!string.IsNullOrWhiteSpace(FirstName))
                {
                    fullName += FirstName;
                }
                if (!string.IsNullOrWhiteSpace(LastName))
                {
                    fullName += " " + LastName;
                }
                return fullName;
            }
        }

        public string JobTitle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Photo { get; set; }

        public string Configuration { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsLockedOut => this.LockoutEnabled && this.LockoutEnd >= DateTimeOffset.UtcNow;

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }


        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        /// <summary>
        /// Demo Navigation property for orders this user has processed
        /// </summary>
        public ICollection<Order> Orders { get; set; }
    }
}
