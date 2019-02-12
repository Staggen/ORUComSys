﻿using Datalayer.Models;
using Datalayer.Repositories;
using Microsoft.AspNet.Identity;
using ORUComSys.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ORUComSys.Controllers {
    [Authorize]
    public class MeetingController : Controller {
        private MeetingRepository meetingRepository;
        private MeetingInviteeRepository meetingInviteeRepository;
        private ProfileRepository profileRepository;

        public MeetingController() {
            ApplicationDbContext context = new ApplicationDbContext();
            meetingRepository = new MeetingRepository(context);
            meetingInviteeRepository = new MeetingInviteeRepository(context);
            profileRepository = new ProfileRepository(context);
        }

        public ActionResult Index() {
            string currentUserId = User.Identity.GetUserId();
            List<MeetingInviteeModels> meetingInvites = meetingInviteeRepository.GetAllMeetingInvitesForUserId(currentUserId);
            List<int> myMeetingIds = meetingInvites.Where((m) => m.ProfileId.Equals(currentUserId)).Select((x) => x.MeetingId).ToList();
            List<MeetingModels> myCreatedMeetings = meetingRepository.GetAllMeetingsByCreatorId(currentUserId);
            List<MeetingModels> myMeetings = meetingRepository.GetMeetingsByMeetingIds(myMeetingIds);

            MeetingViewModels model = new MeetingViewModels {
                ProfileId = currentUserId,
                Invites = meetingInvites,
                MyCreatedMeetings = myCreatedMeetings,
                MyMeetings = myMeetings
            };

            return View(model);
        }

        public ActionResult CreateMeeting() {
            return View();
        }

        [HttpPost]
        public ActionResult CreateMeeting(MeetingModels meeting) {
            if (ModelState.IsValid) {
                MeetingModels model = new MeetingModels {
                    CreatorId = User.Identity.GetUserId(),
                    Title = meeting.Title,
                    Description = meeting.Description,
                    MeetingDateTime = meeting.MeetingDateTime,
                    Location = meeting.Location,
                    Type = meeting.Type
                };
                meetingRepository.Add(model); // Add meeting
                meetingRepository.Save();

                MeetingInviteeModels inviteModel = new MeetingInviteeModels {
                    MeetingId = model.Id,
                    ProfileId = User.Identity.GetUserId(),
                    MeetingAccepted = true
                };
                meetingInviteeRepository.Add(inviteModel); // Invite yourself to the meeting (calendar purposes)
                meetingInviteeRepository.Save();

                return RedirectToAction("MeetingInvitePeople", new { id = model.Id });
            }
            return View();
        }

        public ActionResult EditMeeting(int id) {
            MeetingModels meeting = meetingRepository.Get(id);
            return View(meeting);
        }

        [HttpPost]
        public ActionResult EditMeeting(MeetingModels meeting) {
            if(ModelState.IsValid){
                meeting.CreatorId = User.Identity.GetUserId();
                meetingRepository.Edit(meeting);
                meetingRepository.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("EditMeeting");
        }

        public ActionResult MeetingInvitePeople(int Id) {
            List<ProfileModels> allProfiles = profileRepository.GetAllProfilesExceptCurrent(User.Identity.GetUserId());
            List<MeetingInviteeModels> inviteModels = meetingInviteeRepository.GetAll();
            MeetingInviteViewModels model = new MeetingInviteViewModels {
                MeetingId = Id,
                Profiles = allProfiles.OrderBy((p) => p.FirstName).ToList(),
                Invites = inviteModels
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult AddMeetingInvite(InviteViewModel invite) {
            if (ModelState.IsValid) {
                MeetingInviteeModels model = new MeetingInviteeModels {
                    MeetingId = invite.MeetingId,
                    ProfileId = invite.ProfileId
                };
                meetingInviteeRepository.Add(model);
                meetingInviteeRepository.Save();
                return Json(new { result = true });
            }
            return Json(new { result = false });
        }

        [HttpPost]
        public ActionResult RemoveMeetingInvite(InviteViewModel invite) {
            if (ModelState.IsValid) {
                MeetingInviteeModels model = meetingInviteeRepository.GetMeetingInviteByUserIdAndMeetingId(invite.ProfileId, invite.MeetingId);
                meetingInviteeRepository.Remove(model.Id);
                meetingInviteeRepository.Save();
                return Json(new { result = true });
            }
            return Json(new { result = false });
        }

        [HttpPost]
        public ActionResult AcceptMeetingInvite(int id) {
            if (ModelState.IsValid) {
                MeetingInviteeModels model = meetingInviteeRepository.GetMeetingInviteByUserIdAndMeetingId(User.Identity.GetUserId(), id);
                model.MeetingAccepted = true;
                meetingInviteeRepository.Edit(model);
                meetingInviteeRepository.Save();
                return Json(new { result = true });
            }
            return Json(new { result = false });
        }

        [HttpPost]
        public ActionResult DeclineMeetingInvite(int id) {
            if (ModelState.IsValid) {
                MeetingInviteeModels model = meetingInviteeRepository.GetMeetingInviteByUserIdAndMeetingId(User.Identity.GetUserId(), id);
                meetingInviteeRepository.Remove(model.Id);
                meetingInviteeRepository.Save();
                return Json(new { result = true });
            }
            return Json(new { result = false });
        }
    }
}