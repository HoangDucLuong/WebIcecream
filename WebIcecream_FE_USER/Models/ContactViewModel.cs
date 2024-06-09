namespace WebIcecream_FE_USER.Models
{
    public class ContactViewModel
    {
        public ContactViewModel(string name, string email, string phone, string message) {
            this.name = name;
            this.email = email;
            this.phone = phone;
            this.message = message;
        }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string message { get; set; }
    }
}
