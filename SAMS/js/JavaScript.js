$(document).ready(function () {

    // Create Loader
    $('div#divLoad').jqxLoader({
        width: 110,
        height: 80,
        imagePosition: 'center',
        textPosition: 'bottom',
        isModal: true,
        theme: 'darkblue'
    });

    // Open the loader until we get connection results
    $('#divLoad').jqxLoader('open');

    //Reference to Hub
    myHub = $.connection.myHub;

    // Define functions that can be called on the client
    myHub.client.monitorConnectionResults = function (json) {
        // Close Loader
        $('#divLoad').jqxLoader('close');
        results = $.parseJSON(json);
        if (results.resultType == "error") {
            messageNotification(results.resultType, results.resultMessage, results.resultDuration);
        } else {
            // Store Information on Logged in user in window.loginInfo;
            window.loginInfo = $.parseJSON(json);
        }
    };

    // Request User ID from prompt (unauthenticated user)
    myHub.client.getUserId = function () {
        if (typeof (myHub.server.userId) == 'undefined') {
            $('#divLoad').jqxLoader('close');
            userId = prompt("No authentication detected. Please Enter your ATTUID:", "");
            userId = $.trim(userId).toLowerCase();
            if (userId == "" || userId == null) {
                myHub.client.getUserRecurse();
                return;
            }
            $('#divLoad').jqxLoader('open');
            myHub.server.authenticatedUser(userId);
        }
    };

    // Request userName if it wasn't actually given when requested.
    myHub.client.getUserRecurse = function () {
        myHub.client.getUserId();
    };

    myHub.client.getSessions = function (json) {
        var source =
            {
                datatype: "json",
                datafields:
                    [
                        {
                            name: 'connectionId',
                            type: 'string'
                        },
                        {
                            name: 'userId',
                            type: 'string'
                        },
                        {
                            name: 'userName',
                            type: 'string'
                        },
                        {
                            name: 'sessionStartTime',
                            type: 'date',
                            format: "MM-dd-yyyy HH:mm:ss"
                        },
                        {
                            name: 'flowName',
                            type: 'string'
                        },
                        {
                            name: 'stepName',
                            type: 'string'
                        },
                        {
                            name: 'stepNameStartTime',
                            type: 'date',
                            format: "MM-dd-yyyyTHH:mm:ss"
                        },
                    ],
                id: 'connectionId',
                localdata: json
            };
        var dataAdapter = new $.jqx.dataAdapter(source);
        $("#sashaSessions").jqxGrid({
            altrows: true,
            autoshowloadelement: false,
            columnsautoresize: true,
            columnsreorder: true,
            columnsresize: true,
            deferreddatafields: ['userName', 'userId', 'sessionStartTime', 'stepNameStartTime'],
            filterable: true,
            filtermode: 'excel',
            groupable: true,
            rendered: function () {
                createSessionEvents();
            },
            selectionmode: 'none',
            showgroupsheader: false,
            pageable: true,
            sortable: true,
            source: dataAdapter,
            source: dataAdapter,
            scrollmode: 'deferred',
            showfiltercolumnbackground: false,
            showpinnedcolumnbackground: false,
            showsortcolumnbackground: false,
            theme: 'darkblue',
            width: '95%',
            columns:
                [
                    {
                        text: 'USER NAME',
                        datafield: 'userName',
                        width: 250,
                        align: 'center'
                    },
                    {
                        text: 'ATT ID',
                        datafield: 'userId',
                        width: 100,
                        align: 'center'
                    },
                    {
                        text: 'SESSION START TIME',
                        datafield: 'sessionStartTime',
                        width: 175,
                        cellsformat: 'MM-dd-yyyy HH:mm:ss',
                        align: 'center',
                        cellsalign: 'center'
                    },
                    {
                        text: 'FLOW NAME',
                        datafield: 'flowName',
                        align: 'center'
                    },

                    {
                        text: 'STEP NAME',
                        datafield: 'stepName',
                        align: 'center'
                    },
                    {
                        text: 'STEP START TIME',
                        datafield: 'stepNameStartTime',
                        cellsformat: 'MM-dd-yyyy HH:mm:ss',
                        align: 'center',
                        cellsalign: 'center'
                    },
                ]
        });
    };

    myHub.client.removeRow = function (connectionId) {
        $('#sashaSessions').jqxGrid('deleterow', connectionId);
    };

    myHub.client.addRow = function (connectionId, userName, userId, sessionStartTime, flowName, stepName, stepNameStartTime) {
        $('#sashaSessions').jqxGrid('addrow', connectionId, { userName, userId, sessionStartTime, flowName, stepName, stepNameStartTime }, 'first');
    }

    // Pass it the name of the field and the new value to update
    myHub.client.updateRow = function (connectionId, name, value) {
        data = $('#sashaSessions').jqxGrid('getrowdatabyid', connectionId);
        data[name] = value;
        $('#sashaSessions').jqxGrid('updaterow', connectionId, data);
    }

    // Start the Connection
    $.connection.hub.start()
    .done(function () {
        myHub.server.checkAuthenticated();
    });
});


function messageNotification (messageType, messageText, messageDuration) {
    $('#messageNotificationContent').html(messageText);
    $('div#messageNotification').jqxNotification({
        width: '100%',
        appendContainer: "#notificationContainer",
        opacity: 0.9,
        autoOpen: false,
        closeOnClick: true,
        animationOpenDelay: 800,
        autoClose: true,
        autoCloseDelay: messageDuration,
        template: messageType
    });
    $.connection.hub.stop();
    $('#messageNotification').jqxNotification('open');
    $('#mainContent').html('<div id="divRefreshButton"><input type="button" value="REFRESH" id="refreshButton" /><div>');
    $("#refreshButton").jqxButton({
        width: 120,
        height: 40,
        theme: 'darkblue'
    });
    $("#refreshButton").on('click', function () {
        $('body').html("");
        location.reload();
    });
}

function createSessionEvents() {
    $("#sashaSessions").off('sort filter').on('sort filter', function (event) {
    });

    $('#sashaSessions').off('rowdoubleclick').on('rowdoubleclick', function (event) {
        var args = event.args;
        var boundIndex = args.rowindex;
        data = $('#sashaSessions').jqxGrid('getrowdata', boundIndex);
        console.log(data);
    });
}
