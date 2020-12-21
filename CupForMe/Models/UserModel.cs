using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CupForMe.Models
{
    public class UserModel
    {
        private string primaryRole = string.Empty;
        private string dataAccessRole = string.Empty;
        public UserModel()
        {
            this.Roles = new List<string>();
        }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public string Token { get; set; }
        public string PrimaryRole
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.primaryRole))
                {
                    return this.primaryRole;
                }
                else
                {
                    if (this.Roles.Contains("Administrator"))
                    {
                        return this.Roles.FirstOrDefault(x => x == "Administrator");
                    }
                    else
                    {
                        return this.Roles.FirstOrDefault(x => x == "StandardUser");
                    }
                }
            }
            set
            {
                this.primaryRole = value;
            }
        }

        public string DataAccessRole
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.dataAccessRole))
                {
                    return this.dataAccessRole;
                }
                else
                {
                    if (this.Roles.Contains("FullControl"))
                    {
                        return this.Roles.FirstOrDefault(x => x == "FullControl");
                    }
                    else if (this.Roles.Contains("ReadWrite"))
                    {
                        return this.Roles.FirstOrDefault(x => x == "ReadWrite");
                    }
                    else
                    {
                        return this.Roles.FirstOrDefault(x => x == "ReadOnly");
                    }
                }
            }
            set
            {
                this.dataAccessRole = value;
            }
        }

        public IList<string> Roles { get; set; }

        public string RolesList
        {
            get { return string.Join(", ", this.Roles); }
        }
    }
}
