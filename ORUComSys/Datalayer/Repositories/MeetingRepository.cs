﻿using Datalayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace Datalayer.Repositories {
    public class MeetingRepository : Repository<MeetingModels, int> {
        public MeetingRepository(ApplicationDbContext context) : base(context) { }

        public List<MeetingModels> GetAllMeetingsByCreatorId(string creatorId) {
            return items.Where((m) => m.CreatorId.Equals(creatorId)).ToList();
        }
    }
}