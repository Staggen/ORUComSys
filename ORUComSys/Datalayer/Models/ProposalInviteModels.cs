﻿using Datalayer.Repositories;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Datalayer.Models {
    public class ProposalInviteModels : IIdentifiable<int> {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Proposal")]
        public int ProposalId { get; set; }
        public virtual ProposedMeetingModels Proposal { get; set; }

        [Required]
        [ForeignKey("Profile")]
        public string ProfileId { get; set; }
        public virtual ProfileModels Profile { get; set; }

        public DateTime NotificationDateTime { get; set; }
        
        public DateTime ProposedDateTime { get; set; }

        [Required]
        public bool Accepted { get; set; } = false;
    }
}