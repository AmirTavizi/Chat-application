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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class LoginHistory : BaseEntity
    {
        public string Ip { get; set; }
        
        public string Description { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public bool Success { get; set; } = true;
        public User User { get; set; }

        

    }
}
