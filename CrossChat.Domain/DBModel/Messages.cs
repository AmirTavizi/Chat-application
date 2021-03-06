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

    public partial class Message : BaseEntity
    {

        
        public string MessageBody { get; set; }
        
        public Guid SourceUserId { get; set; }
       
        public Guid? DestinationUserId { get; set; }
        [ForeignKey("Channel")]
        public Guid? ChannelId { get; set; }       
        [ForeignKey("MessageType")]
        public Guid? MessageTypeId { get; set; }
        
        public User SourceUser { get; set; }
        public User DestinationUser { get; set; }
        public Channel Channel { get; set; }
        public MessageType MessageType { get; set; }


    }
}
