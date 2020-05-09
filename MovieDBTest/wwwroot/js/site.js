// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//****************************************************************
//*************** Setzen von Gesehen - Status ********************
//****************************************************************
var sendNewWatchedStatus = function(_movieId, _isSetToWatched) {

    var data = JSON.stringify({
        movieId: _movieId,
        isSetToWatched: _isSetToWatched,
    });
        
    var request = $.ajax({
        type: "GET",
        url: "/Interaction/SetMovieWatchedStatus?movieId=" + _movieId + "&isSetToWatched=" + _isSetToWatched,
    });
}

var WatchedBoxClick = function(movieId, checkbox) {
    if (checkbox.is(':checked')) {
        sendNewWatchedStatus(movieId, true);
    }
    else {
        sendNewWatchedStatus(movieId, false);
    }
}

//****************************************************************
//*************** Bucketlist Edit View Shit * ********************
//****************************************************************

//einen eingetragenen Nutzer suchen und eintragen
var getExistingUser = function() {

    //Fehlermeldung erstmal ausblenden
    $('#UserError').addClass("hide");

    //Username aus textfeld ziehen
    var username = $('#AddUserNameText').val();

    var request = $.ajax({
        type: "GET",
        url: "/Interaction/GetExistingUser?username=" + username,
        //Funktion bei technischem Erfolg (200er Rückmeldung)
        success: function (data) {
            //Wenn leerer String zurück kommt -> User existiert nicht
            if (data != "") {
                //User-Daten parsen
                var userData = JSON.parse(data);
                //neuen User anhängen und eventhandler aktualisieren
                $("#UsersInBucketList").append(createUserString(userData.Name, userData.Id));
                $("[name='DeleteButton']").click(removeUser);
            }
            else {
                //wenn kein User -> fehlermeldung zeigen
                $('#UserError').removeClass("hide");
            }
        },
        //Bei technischem Fehler -> billige Meldung
        error: function (data) {
            alert("Error: "+ data);
        },
    });
}

//direktes Parent-Objekt entfernen 
var removeUser = function () {
    $(this).parent().remove();
}

//HTML-String für neuen nutzen in BucketListEditView
var createUserString = function (userName, userId) {
    return "<div name=\"singleUser\"  id=\"" + userId + "\"> <p>" + userName + "</p><input type=\"button\" value=\"Remove\" name=\"DeleteButton\"></div> "
}

var getMovieByTitle = function () {
    //Filmtitel aus textfeld ziehen
    var title = $('#AddMovieTitleText').val();

    var request = $.ajax({
        type: "GET",
        url: "/Interaction/GetNewBucketListMovie?title=" + title,
        //Funktion bei technischem Erfolg (200er Rückmeldung)
        success: function (data) {
            //Wenn leerer String zurück kommt -> Film nicht gefunden 
            if (data != "") {
                //partial View oben anfügen 
                $("#MovieList").prepend(data);
                $("[name='RemoveMovieButton']").click(removeMovie);
            }
            else {
                //kein Film -> fehlermeldung zeigen
                alert("Error");
            }
        },
        //Bei technischem Fehler -> billige Meldung
        error: function (data) {
            alert("Error: " + data);
        },
    });
}

//löscht Filmeintrag
var removeMovie = function () {
    $(this).parent().parent().remove();
}



var saveBucketList = function () {

    var request = $.ajax({
        type: "POST",
        url: "/Interaction/SaveBucketList",
        data: getNewCreateModel(),
        contentType: "application/json",
        //Funktion bei technischem Erfolg (200er Rückmeldung)
        success: function (data) {
            //Wenn leerer String zurück kommt -> Film nicht gefunden 
            if (data != "") {
                //partial View oben anfügen 
                $("#MovieList").prepend(data);
                $("[name='RemoveMovieButton']").click(removeMovie);
            }
            else {
                //kein Film -> fehlermeldung zeigen
                alert("Error");
            }
        },
        //Bei technischem Fehler -> billige Meldung
        error: function (data) {
            alert("Error: " + data);
        },
    });
}


var getNewCreateModel = function () {
    var _ListId = $("#ListId").val();
    var _ListTitle = $("#ListTitle").val();

    var _UserIds = [];
    var _MovieImdbsId = [];

    $("[name='MovieModel']").each(function () {
        _MovieImdbsId.push($(this).attr("id"));
    });

    $("[name='singleUser']").each(function () {
        _UserIds.push($(this).attr("id"));
    });

    var response = {
        ListId: _ListId,
        ListTitle: _ListTitle,
        UserIds: _UserIds,
        MovieImdbIds: _MovieImdbsId
    }

    return JSON.stringify(response);
}


$('#SaveButton').click(saveBucketList);


$('#AddUserButton').click(getExistingUser);
$('#AddMovieTitleButton').click(getMovieByTitle);
$("[name='DeleteButton']").click(removeUser);
$("[name='RemoveMovieButton']").click(removeMovie);