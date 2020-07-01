using System;

namespace syncdaemon
{
    public partial class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public DateTime? Dob { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string CellPhone { get; set; }
        public string OtherPhone { get; set; }
        public string Email { get; set; }
        public string PreferredContact { get; set; }
        public string PreferredContactType { get; set; }
        public string Suffix { get; set; }
        public int AccountId { get; set; }
        public DateTime? TodaysDate { get; set; }
    }
}
