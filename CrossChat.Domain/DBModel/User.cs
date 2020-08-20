//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CrossChat.Domain.DBModel
{

    using System.Text.Json.Serialization;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System;

    public partial class User : BaseEntity
    {
        [Required(ErrorMessage = "Please Enter Name")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please Enter Surname")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Surname")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Please Enter Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [NotMapped]
        [CompareAttribute("Password", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPassowrd { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Avatar")]
        public string Avatar { get; set; }
        public DateTime SendHashCodeDateTime { get; set; }

        public string ChangePasswordHashCode { get; set; }
        public bool IsOpenToOthersAddChannels { get; set; } = true;
        [JsonIgnore]
        public List<LoginHistory> LoginHistories { get; set; }
  
        [JsonIgnore]
        public List<MemberShip> MemberShips { get; set; }
        [NotMapped]
        public DateTime? LatestMessageDateTime { get; set; }
        [JsonIgnore]
        public List<Message> SendedMessage { get; set; } 
        [JsonIgnore]
        public List<Message> ReciveMessage { get; set; }

    }
}
