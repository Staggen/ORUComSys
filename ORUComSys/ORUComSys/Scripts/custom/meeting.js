﻿$(document).ready(() => {
    console.log("Script loaded: meeting.js");
});

$("body").on("click", ".accept-meeting-invite-button", AcceptInvite);
$("body").on("click", ".decline-meeting-invite-button", DeclineInvite);
$("body").on("click", ".btn-participant-list", ToggleParticipantsDiv);
$("body").on("click", "#delete-meeting-btn", DeleteMeeting);

function AcceptInvite() {
    var meetingId = $(this).data("meeting-id");
    $.ajax({
        url: "/Meeting/AcceptMeetingInvite/" + meetingId,
        type: "POST",
        dataType: "JSON",
        success: () => {
            // Set notifications
            GetNumberOfNotifications();
            RefreshNotificationsContent();
            // Set other UI elements
            var counter = $($(this).parents().eq(1)[0].childNodes[3]).text();
            $($(this).parents().eq(1)[0].childNodes[3]).text(parseInt(counter, 10) + 1);
            SetBorders($(this).parents().eq(2));
            ButtonGroup($(this).parents().eq(0));
        },
        error: () => {
            console.log("Error: Unable to accept invite");
        }
    });
}

function DeclineInvite() {
    var meetingId = $(this).data("meeting-id");
    $.ajax({
        url: "/Meeting/DeclineMeetingInvite/" + meetingId,
        type: "POST",
        dataType: "JSON",
        success: () => {
            $(this).parents().eq(2).addClass("d-none"); // KKona
        },
        error: () => {
            console.log("Error: Unable to decline invite.");
        }
    });
}

function ButtonGroup(elementId) {
    if (!$(elementId).hasClass("d-none")) {
        $(elementId).addClass("d-none");
    }
}

function SetBorders(entireElement) {
    if ($(entireElement[0]).hasClass("border-warning")) {
        $(entireElement).removeClass("border-warning");
    }
    if (!$(entireElement[0]).hasClass("border-success")) {
        $(entireElement).addClass("border-success");
    }
    if (!$(entireElement[0].childNodes[1]).hasClass("border-bottom")) {
        $(entireElement[0].childNodes[1]).addClass("border-bottom");
    }
    if (!$(entireElement[0].childNodes[1]).hasClass("border-success")) {
        $(entireElement[0].childNodes[1]).addClass("border-success");
    }
    if (!$(entireElement[0].childNodes[5]).hasClass("border-top")) {
        $(entireElement[0].childNodes[5]).addClass("border-top");
    }
    if (!$(entireElement[0].childNodes[5]).hasClass("border-success")) {
        $(entireElement[0].childNodes[5]).addClass("border-success");
    }
}

function ToggleParticipantsDiv() {
    if ($(this).hasClass("focus")) {
        $(this).removeClass("focus");
    }
    if ($("#ParticipantsDiv").hasClass("d-none")) {
        GetParticipantsContent(this);
    } else {
        ToggleParticipantsDisplay();
    }
}

function GetParticipantsContent(passedThis) {
    var meetingId = $(passedThis).data("meeting-id");
    $.ajax({
        url: "/Meeting/GetParticipantsContent/" + meetingId,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "html",
        success: function (data) {
            $("#ParticipantsDiv").html(data);
            CreateParticipantsPopper(passedThis);
        },
        error: () => {
            console.log("Error: Unable to get participants content.");
        }
    });
}

function CreateParticipantsPopper(passedThis) {
    var ref = passedThis;
    var pop = $("#ParticipantsDiv");
    new Popper(ref, pop, {
        placement: "top",
        modifiers: {
            offset: {
                enabled: true,
                offset: "0, 10"
            }
        }
    });
    ToggleParticipantsDisplay();
}

function ToggleParticipantsDisplay() {
    $("#ParticipantsDiv").toggleClass("d-none");
}

function DeleteMeeting() {
    var meetingId = this.getAttribute("data-meeting-id");
    var leftMeetingDisplay = $("#meeting-div-" + meetingId);
    $.ajax({
        type: "DELETE",
        url: "/api/AjaxApi/DeleteMeeting/" + meetingId,
        contentType: "application/json;charset=UTF-8",
        success: () => {
            $($(this).parents()[1]).addClass("d-none");
            leftMeetingDisplay.addClass("d-none");
        },
        error: () => {
            alert("Error: Failure to delete meeting");
        }
    });
}