using Asset_Management.Models.SQL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Asset_Management.Models.LDAP
{
    /// <summary>
    ///
    /// </summary>
    public class ResultDataGet : DefaultResponse, IDefaultResponse
    {
        public bool Found { get; set; }
        public object Data { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class UserInfo
    {
        public bool AccountEnabled { get; set; }
        public string BadPasswordTime { get; set; }
        public string BadPwdCount { get; set; }
        public string CanonicalName { get; set; }
        public List<string> DirectReports { get; } = new List<string>();
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public string GivenName { get; set; }
        public string HomeDirectory { get; set; }
        public string Initials { get; set; }
        public string LastBadPasswordAttempt { get; set; }
        public string LastLogonTimestamp { get; set; }
        public string Mail { get; set; }
        public List<string> ManagedObjects { get; } = new List<string>();
        public List<string> MemberOf { get; } = new List<string>();
        public string PasswordAge { get; set; }
        public string PasswordNeverExpires { get; set; }
        public List<string> PublicDelegatesBl { get; } = new List<string>();
        public string PwdLastSet { get; set; }
        public string SamAccountName { get; set; }
        public string ScriptPath { get; set; }
        public string Sn { get; set; }
        public string Uid { get; set; }
        public string UserAccountControl { get; set; }
        public string UserPrincipalName { get; set; }
        public string WhenChanged { get; set; }
        public string WhenCreated { get; set; }
    }

    public class User
    {
        private readonly IConfiguration _configuration;

        public User(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public User()
        {

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="userName"></param>
        /// <param name="domain"></param>
        /// <param name="environment"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ResultDataGet GetUserAttributes(string userName, AssetContext context)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));

            var start = DateTime.UtcNow;
            var retVal = new ResultDataGet
            {
                Execution_time = start.ToString("yyyy-MM-ddTHH:mm:ssZ", new CultureInfo("en-US", false))
            };

            var attributes = new UserInfo();

            if (!Strings.Base64Model.TryParseBase64(_configuration["LDAP:Password"], Encoding.UTF8, out var prodPassword))
            {
                prodPassword = "";
            }

            var domain = _configuration["LDAP:Domain"];
            var username = _configuration["LDAP:Username"];

            try
            {
                DirectoryEntry root = new DirectoryEntry(domain, username, prodPassword);


                using (var search = new DirectorySearcher(root))
                {
                    search.Filter = string.Format(new CultureInfo("en-US", false), "(&(objectCategory=user)(sAMAccountName={0}))", userName);

                    search.PropertiesToLoad.AddRange(new string[] {
                        "BadPasswordTime",
                        "BadPwdCount",
                        "CanonicalName",
                        "DirectReports",
                        "DisplayName",
                        "DistinguishedName",
                        "GivenName",
                        "HomeDirectory",
                        "Initials",
                        "LastBadPasswordAttempt",
                        "LastLogonTimestamp",
                        "Mail",
                        "ManagedObjects",
                        "MemberOf",
                        "PasswordAge",
                        "PublicDelegatesBl",
                        "PwdLastSet",
                        "SamAccountName",
                        "ScriptPath",
                        "Sn",
                        "Uid",
                        "UserAccountControl",
                        "UserPrincipalName",
                        "WhenChanged",
                        "WhenCreated"
                    });

                    var result = search.FindOne();

                    if (result != null)
                    {
                        var thisDay = DateTime.UtcNow;

                        if (result.Properties.Contains("BadPasswordTime"))
                        {
                            var datenew = Convert.ToInt64(result.Properties["BadPasswordTime"][0].ToString(), new CultureInfo("en-US", false));
                            var daysNewNow = new DateTime(1601, 01, 01, 0, 0, 0, DateTimeKind.Utc).AddTicks(datenew);
                            attributes.BadPasswordTime = daysNewNow.ToString("yyyy-MM-ddTHH:mm:ssZ", new CultureInfo("en-US", false));
                        }
                        else
                        {
                            attributes.BadPasswordTime = "Never";
                        }

                        if (result.Properties.Contains("badPwdCount"))
                        {
                            attributes.BadPwdCount = result.Properties["badPwdCount"][0].ToString();
                        }
                        else
                        {
                            attributes.BadPwdCount = "0";
                        }

                        if (result.Properties.Contains("CanonicalName"))
                        {
                            attributes.CanonicalName = result.Properties["CanonicalName"][0].ToString();
                        }
                        else
                        {
                            attributes.CanonicalName = "N/A";
                        }

                        if (result.Properties.Contains("DirectReports"))
                        {
                            attributes.DirectReports.AddRange(result.Properties["DirectReports"].Cast<string>().ToList());
                        }
                        else
                        {

                        }

                        if (result.Properties.Contains("DisplayName"))
                        {
                            attributes.DisplayName = result.Properties["DisplayName"][0].ToString();
                        }
                        else
                        {
                            attributes.DisplayName = "N/A";
                        }

                        if (result.Properties.Contains("DistinguishedName"))
                        {
                            attributes.DistinguishedName = result.Properties["DistinguishedName"][0].ToString();
                        }
                        else
                        {
                            attributes.DistinguishedName = "N/A";
                        }


                        if (result.Properties.Contains("givenname"))
                        {
                            attributes.GivenName = result.Properties["givenname"][0].ToString();
                        }
                        else
                        {
                            attributes.GivenName = "N/A";
                        }

                        if (result.Properties.Contains("HomeDirectory"))
                        {
                            attributes.HomeDirectory = result.Properties["HomeDirectory"][0].ToString();
                        }
                        else
                        {
                            attributes.HomeDirectory = "N/A";
                        }

                        if (result.Properties.Contains("initials"))
                        {
                            attributes.Initials = result.Properties["initials"][0].ToString();
                        }
                        else
                        {
                            attributes.Initials = "N/A";
                        }

                        if (result.Properties.Contains("badPasswordTime"))
                        {
                            var date = Convert.ToInt64(result.Properties["badPasswordTime"][0].ToString(), new CultureInfo("en-US", false));

                            if (date != 0)
                            {
                                var daysNew = new DateTime(1601, 01, 01, 0, 0, 0, DateTimeKind.Utc).AddTicks(date);
                                attributes.LastBadPasswordAttempt = daysNew.ToString("yyyy-MM-ddTHH:mm:ssZ", new CultureInfo("en-US", false));
                            }
                            else
                            {
                                attributes.LastBadPasswordAttempt = "N/A";
                            }
                        }
                        else
                        {
                            attributes.LastBadPasswordAttempt = "N/A";
                        }

                        if (result.Properties.Contains("lastlogontimestamp"))
                        {
                            var datenew = Convert.ToInt64(result.Properties["lastlogontimestamp"][0].ToString(), new CultureInfo("en-US", false));
                            var daysNewNow = new DateTime(1601, 01, 01, 0, 0, 0, DateTimeKind.Utc).AddTicks(datenew);
                            attributes.LastLogonTimestamp = daysNewNow.ToString("yyyy-MM-ddTHH:mm:ssZ", new CultureInfo("en-US", false));
                        }
                        else
                        {
                            attributes.LastLogonTimestamp = "Never";
                        }

                        if (result.Properties.Contains("mail"))
                        {
                            attributes.Mail = result.Properties["mail"][0].ToString();
                        }
                        else
                        {
                            attributes.Mail = "N/A";
                        }

                        if (result.Properties.Contains("managedobjects"))
                        {
                            attributes.ManagedObjects.AddRange(result.Properties["managedobjects"].Cast<string>().ToList());
                        }
                        else
                        {

                        }

                        if (result.Properties.Contains("memberof"))
                        {
                            attributes.MemberOf.AddRange(result.Properties["memberof"].Cast<string>().ToList());
                        }
                        else
                        {

                        }

                        if (result.Properties.Contains("userAccountControl"))
                        {
                            var DONT_EXPIRE_PASSWORD = 0x10000;

                            var ACCOUNTDISABLE = 0x0002;

                            var value = (int)result.Properties["userAccountControl"][0];

                            if ((value | ACCOUNTDISABLE).Equals(value))
                            {
                                attributes.AccountEnabled = false;
                            }
                            else
                            {
                                attributes.AccountEnabled = true;
                            }

                            if ((value | DONT_EXPIRE_PASSWORD).Equals(value))
                            {
                                attributes.PasswordAge = "The password does not expire.";
                                attributes.PasswordNeverExpires = "True";
                            }
                            else
                            {
                                var date = Convert.ToInt64(result.Properties["pwdLastSet"][0].ToString(), new CultureInfo("en-US", false));
                                var daysNew = new DateTime(1601, 01, 01, 0, 0, 0, DateTimeKind.Utc).AddTicks(date);

                                var baseDate = new DateTime(1601, 01, 01, 0, 0, 0, DateTimeKind.Utc);

                                if (daysNew.Equals(baseDate))
                                {
                                    attributes.PasswordAge = "No Set Time";
                                }
                                else
                                {
                                    var daysOld = (thisDay - daysNew).TotalDays;
                                    var days = Math.Round(daysOld, 0);
                                    attributes.PasswordAge = string.Format(new CultureInfo("en-US", false), "{0} Days", days.ToString(new CultureInfo("en-US", false)));
                                }

                                attributes.PasswordNeverExpires = "False";
                            }
                        }
                        else
                        {
                            attributes.PasswordAge = "Unknown";
                        }

                        if (result.Properties.Contains("publicdelegatesbl"))
                        {
                            attributes.PublicDelegatesBl.AddRange(result.Properties["publicdelegatesbl"].Cast<string>().ToList());
                        }
                        else
                        {

                        }

                        if (result.Properties.Contains("pwdLastSet"))
                        {
                            var date = Convert.ToInt64(result.Properties["pwdLastSet"][0].ToString(), new CultureInfo("en-US", false));

                            if (date != 0)
                            {
                                var daysNew = new DateTime(1601, 01, 01, 0, 0, 0, DateTimeKind.Utc).AddTicks(date);
                                attributes.PwdLastSet = daysNew.ToString("yyyy-MM-ddTHH:mm:ssZ", new CultureInfo("en-US", false));
                            }
                            else
                            {
                                attributes.PwdLastSet = "Never";
                            }
                        }
                        else
                        {
                            attributes.PwdLastSet = "N/A";
                        }

                        if (result.Properties.Contains("SamAccountName"))
                        {
                            attributes.SamAccountName = result.Properties["SamAccountName"][0].ToString();
                        }
                        else
                        {
                            attributes.SamAccountName = "N/A";
                        }

                        if (result.Properties.Contains("scriptPath"))
                        {
                            attributes.ScriptPath = result.Properties["scriptPath"][0].ToString();
                        }
                        else
                        {
                            attributes.ScriptPath = "N/A";
                        }

                        if (result.Properties.Contains("sn"))
                        {
                            attributes.Sn = result.Properties["sn"][0].ToString();
                        }
                        else
                        {
                            attributes.Sn = "N/A";
                        }

                        if (result.Properties.Contains("uid"))
                        {
                            attributes.Uid = result.Properties["uid"][0].ToString();
                        }
                        else
                        {
                            attributes.Uid = "N/A";
                        }

                        if (result.Properties.Contains("userPrincipalName"))
                        {
                            attributes.UserPrincipalName = result.Properties["userPrincipalName"][0].ToString();
                        }
                        else
                        {
                            attributes.UserPrincipalName = "N/A";
                        }

                        if (result.Properties.Contains("whenChanged"))
                        {
                            var date = result.Properties["whenChanged"][0].ToString();

                            if (!string.IsNullOrEmpty(date))
                            {
                                var createDate = DateTime.Parse(date, new CultureInfo("en-US", false));
                                attributes.WhenChanged = createDate.ToString("yyyy-MM-ddTHH:mm:ssZ", new CultureInfo("en-US", false));
                            }
                            else
                            {
                                attributes.WhenChanged = "N/A";
                            }
                        }
                        else
                        {
                            attributes.WhenChanged = "N/A";
                        }

                        if (result.Properties.Contains("whenCreated"))
                        {
                            var date = result.Properties["whenCreated"][0].ToString();

                            if (!string.IsNullOrEmpty(date))
                            {
                                var createDate = DateTime.Parse(date, new CultureInfo("en-US", false));
                                attributes.WhenCreated = createDate.ToString("yyyy-MM-ddTHH:mm:ssZ", new CultureInfo("en-US", false));
                            }
                            else
                            {
                                attributes.WhenCreated = "N/A";
                            }
                        }
                        else
                        {
                            attributes.WhenCreated = "N/A";
                        }

                        retVal.Success = true;
                        retVal.Found = true;
                        retVal.Message = "Account found";
                    }
                    else
                    {
                        retVal.Success = true;
                        retVal.Found = false;
                        retVal.Message = "Account not found";
                    }
                }
                root.Dispose();
            }
            catch (Exception ex)
            {
                /*
                LogEntryModel.InsertToDB(new LogData()
                {
                    CalledUrl = "N/A",
                    CallerIp = "N/A",
                    EventTime = DateTime.UtcNow,
                    EventType = "Query Error",
                    Message = "An error occured while attempting to query " + environment + " Active Directory - GetUserAttributes",
                    Server = Environment.MachineName,
                    Location = "API",
                    UserAgent = "N/A",
                    UserId = "N/A",
                    EventLevel = "ERROR",
                    Exception = ex.InnerException?.Message ?? ex.Message ?? ex.ToString()
                }, context, cancellationToken);
                */
                retVal.Success = false;
                retVal.Found = false;
                retVal.Message = ex.InnerException?.Message ?? ex.Message ?? ex.ToString();
            }

            retVal.Data = attributes;
            retVal.Execution_duration = (DateTime.UtcNow - start).TotalSeconds.ToString(new CultureInfo("en-US", false));
            return retVal;
        }
    }
}
