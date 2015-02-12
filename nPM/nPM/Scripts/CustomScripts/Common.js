
$(document).ready(function () {

    debugger;

    var entity = $('#hdEntity').val();
    var page = $('#hdPage').val();
    var info = $('#hdInfo').val();   
    if (page == "Index") {
        hide_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
        hide_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
    }
    if (page == "Edit" || page == "Create") {
        hide_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
        hide_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
        hide_ribbon_icon('New ' + entity, 'tdNew', 'innertdNew');
    }
    function load() {
        $('#dvLoading').show();
        var opts = {
            lines: 17, // The number of lines to draw
            length: 20, // The length of each line
            width: 4, // The line thickness
            radius: 30, // The radius of the inner circle
            corners: 1, // Corner roundness (0..1)
            rotate: 79, // The rotation offset
            direction: 1, // 1: clockwise, -1: counterclockwise
            color: '#000', // #rgb or #rrggbb or array of colors
            speed: 1, // Rounds per second
            trail: 60, // Afterglow percentage
            shadow: false, // Whether to render a shadow
            hwaccel: false, // Whether to use hardware acceleration
            className: 'spinner', // The CSS class to assign to the spinner
            zIndex: 2e9, // The z-index (defaults to 2000000000)
            top: 'auto', // Top position relative to parent in px
            left: 'auto' // Left position relative to parent in px
        };
        var target = document.getElementById('dvLoading');
        var spinner = new Spinner(opts).spin(target);
        target.appendChild(spinner.el);
    }

    function show_ribbon_icon(iconText, tdId, innertdId) {
        $('#' + tdId).find('table').attr('class', 'tbl_ribbon_right');
        $('#' + innertdId).text(iconText);
        $('#' + tdId).show();
    }

    function hide_ribbon_icon(iconText, tdId, innertdId) {
        $('#' + tdId).find('table').attr('class', 'tbl_ribbon_right_hidden');
        $('#' + innertdId).text('');
        $('#' + tdId).hide();
    }

    function create_selection_table_with_no_cbox(array, ttblId, tblTitle) {
        $('#' + ttblId + '').innerHTML = "";
        //add table 's header
        var header =
                    '   <thead>' +
                    '   <tr>' +
                    '   <th style="width:10px"></th>' +
                    '   <th >' + tblTitle + '</th>' +
                    '   </tr>' +
                    '   </thead>';
        $('#' + ttblId + '').append(header);
        $('#' + ttblId + '').append('<tbody>');
        //add table's body
        for (var i = 0; i < array.length; i++) {
            $('#' + ttblId + '').append(
                '<tr><td style="width:10px"><input type="hidden"  id="' + array[i].Id + '"/></td>' + 
                '<td><a class="btn_link"href="/Task/gotoUpdateTaskForm?record_id=' + array[i].Id + '" id="' + array[i].Name + '">' + array[i].Name + '</a></td></tr>');
        }
        $('#' + ttblId + '').append('</tbody>');
    }   

    function create_selection_table(array, ttblId, tblTitle) {
        //add table 's header
        var header =
                    '   <thead>' +
                    '   <tr>' +
                    '   <th style="width:10px"><input id="cboxAll" type="checkbox" name="cboxAll" /></th>' +
                    '   <th >' + tblTitle + '</th>' +
                    '   </tr>' +
                    '   </thead>';
        $('#' + ttblId + '').append(header);
        $('#' + ttblId + '').append('<tbody>');
        //add table's body
        for (var i = 0; i < array.length; i++) {
            $('#' + ttblId + '').append('<tr><td style="width:10px"><input type="checkbox" name="cbox" value="' + array[i].Id + '"/></td><td>' + array[i].Name + '</td></tr>');
        }
        $('#' + ttblId + '').append('</tbody>');
    }

    $('.tbl_entity_list').find('tr').dblclick(function () {

        try {
            var selectedId = $(this).find('input[name="cboxOne"]').val();
            $.ajax({
                type: "get",
                url: "/" + entity + "/gotoUpdate" + entity + "Form",
                data: { entityId: selectedId },
                beforeSend: function (xhr) {
                    load();
                },
                success: function (data) {
                    $('#dvLoading').hide();
                    $("#dvSectionRight").html(data);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $('#dvLoading').hide();
                    error_handlerXML('Error occured while open SR', XMLHttpRequest);
                }
            });
        }
        catch (e) {
            toastr.error(e.message);
        }


    });

    //select all checkbox on click
    $('#cboxAll').click(function () {
        if (this.checked == true) {
            show_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
            $('input[name = "cboxOne"]').each(function () {
                this.checked = true;
            });
            hide_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
            hide_ribbon_icon('Delete ' + entity, 'tdNew', 'innertNew');
        }
        else {
            hide_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
            hide_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
            show_ribbon_icon('Delete ' + entity, 'tdNew', 'innertNew');
            $('input[name = "cboxOne"]').each(function () {
                this.checked = false;
            });
        }
    });
    //show or hide Edit and Delete button on click of checkbox
    $('input[name="cboxOne"]').click(function () {

        var entity = $('#hdEntity').val();
        var cboxes = $('input[name="cboxOne"]:checked');
        if (cboxes.length == 1) {
            show_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
            show_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
            hide_ribbon_icon('Delete ' + entity, 'tdNew', 'innertNew');
        }
        else if (cboxes.length > 0) {
            hide_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
            show_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
            hide_ribbon_icon('New ' + entity, 'tdNew', 'innertNew');
        }
        else {
            hide_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
            hide_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
            show_ribbon_icon('Delete ' + entity, 'tdNew', 'innertNew');
        }
    });

    //go to create User Page
    $('#Add').click(function () {
        debugger;
        var entity = $('#hdEntity').val();
        var page = $('#hdPage').val();
        try {
            $.ajax({
                type: "get",
                url: "/" + entity + "/gotoCreate" + entity + "Form",
                beforeSend: function (xhr) {
                    //load();
                },
                success: function (data) {
                    $("#dvSectionRight").html(data);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $('#dvLoading').hide();
                    error_handlerXML('Error occured while opening add ' + entity + ' Page', XMLHttpRequest);
                }
            });
        }
        catch (e) {
            $('#dvLoading').hide();
            toastr.error(e.message);
        }
    });

    $('#Save').click(function () {
        
        //if (page == "Update") {
        //    $('#frmUpdate' + entity + '').submit();
        //}
        $('#frmCreate' + entity + '').submit();
    });

    $('#Send').click(function () {
        $('#frmCreate' + entity + '').submit();
    });

    $('#Update').click(function () {
        $('#frmUpdate' + entity + '').submit();
    });

    /*$('#Edit').click(function () {
        try {
            //selected record id
            debugger;
            var entity = $('#hdEntity').val();
            var s_record_id = "00000000-0000-0000-0000-000000000000";
            var count = 0;
            $("input:checkbox[name=cboxOne]:checked").each(function () {
                s_record_id = $(this).val();
                count++;
            });
            if (count != 1) {
                toastr.warning("You can only edit on record at the time.");
                return null;
            }
            if (count == 1) {
                $.ajax({
                    type: "get",
                    url: "/" + entity + "/gotoUpdate" + entity + "Form",
                    data: { record_id: s_record_id },
                    beforeSend: function (xhr) {
                        //load();
                    },
                    success: function (data) {
                        $("#dvSectionRight").html(data);
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        $('#dvLoading').hide();
                        error_handlerXML('Error occured while opening ' + entity + ' User Page', XMLHttpRequest);
                    }
                });
            }
        }
        catch (e) {
            $('#dvLoading').hide();
            toastr.error(e.message);
        }
    });
    */
    /*$('#imgAddRole').click(function () {
        try {
            debugger
            $.ajax({
                type: "get",
                url: "/User/getAllRoles",
                success: function (data) {
                    $("#tblAnyEntity tr").remove();
                    create_selection_table(data.success, "tblAnyEntity", "Roles");
                    var roleTable = $('#tblAnyEntity').dataTable({
                            "iDisplayLength": 20,
                            "aLengthMenu": [[10, 20, 50, -1], [10, 20, 50, "All"]],
                            "sPaginationType": "full_numbers"
                    });
                    roleTable.fnSort([[1, 'asc']]);
                      
                    //});
                    $('#dv_popup').show();
                    $('#dvOverlay').show();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    error_handlerXML('Error occured while fetchin projects', XMLHttpRequest);
                }
            });
        }
        catch (e) {
            toastr.error(e.message);
        }

    });*/

    $('#btnCancel').click(function () {
        $('#dv_popup').hide();
        $('#dvOverlay').hide();
    });

    $('#btnOk').click(function () {
        try {
            //get all check boxes with name: cboxOne that are checked
            //debugger
            var cbox = $('input[name="cbox"]:checked');
            var selectedUserIds = new Array();
            for (var i = 0; i < cbox.length; i++) {
                selectedUserIds.push(cbox.val());
            }
            var record_id = $('#Id').val();
            $('#dvOverlay').hide();
            $('#dv_popup').hide();
            $.ajax({
                type: "get",
                url: "/User/updateRoles",
                data: { roles: selectedUserIds, user_id: record_id },
                traditional: true,
                success: function (data) {
                    
                    $("#dvSectionRight").html(data);
                    toastr.success("Role added");
                    
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    error_handlerXML('Error occured while open SR', XMLHttpRequest);
                }
            });
        }
        catch (e) {
            toastr.error(e.message);
        }
    });

    $('#Assign').click(function () {
        var TaskId = $('#Id').val();
        var TaskName = $('#Name').val();
        $.ajax({
            type: "get",
            url: "/Task/gotoAssignUserForm",
            data: { task_id: TaskId },
            traditional: true,
            success: function (data) {
                $("#dvNothing").html(data);
                $('#TaskId').val(TaskId);
                $('#TaskName').val(TaskName);
                //$('#dvOverlay').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while open SR', XMLHttpRequest);
            }
        });
    });

    $('#Link').click(function () {
        var TaskId = $('#Id').val();
        var TaskName = $('#Name').val();
        $.ajax({
            type: "get",
            url: "/Task/gotoLinkTaskForm",
            data: { task_id: TaskId },
            traditional: true,
            success: function (data) {
                $("#dvNothing").html(data);
                $('#TaskId1').val(TaskId);
                $('#TaskName1').val(TaskName);
                //$('#dvOverlay').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while open SR', XMLHttpRequest);
            }
        });
    });

    //toggle content
    $('.dv_group').click(function () {
        //dvId will contain id of div that is being clicked
        debugger;
        var dvId = "#dv" + $(this).text() + "Container";
        $(dvId).toggle('slow');
        
    });

    $('.dv_groupitem').click(function () {

        debugger;
        try {


            //dvText will contain controller's name
            var dvText = $(this).find('h2').text();
            if (dvText == "Users") {
                dvText = "User";
            }
            //$('.dv_groupitem').attr("class", "dv_groupitem_clicked");
            $('#h2CurrentEntityName').text(entity);
        }
        catch (e) {
            $('#dvLoading').hide();
            toastr.error("dv_groupitem clicked: " + e.message);
        }
    });
    $('#dvDocument').click(function () {
        debugger;
        if (page == "Index") {
            hide_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
            hide_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
        }
    });
    $('#dvUser').click(function () {
        debugger;
        if (page == "Index") {
            hide_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
            hide_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
        }
    });
    $('#dvClient').click(function () {
        debugger;
        if (page == "Index") {
            hide_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
            hide_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
        }
    });
    $('#dvTask').click(function () {
        debugger;
        if (page == "Index") {
            hide_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
            hide_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
        }
    });
    $('#dvProject').click(function () {
        debugger;
        if (page == "Index") {
            hide_ribbon_icon('Edit ' + entity, 'tdEdit', 'innertdEdit');
            hide_ribbon_icon('Delete ' + entity, 'tdDelete', 'innertdDelete');
        }
    });
    
    $('#imgAddProjectTask').click(function () {
        debugger;
        var prjctId = $('#Id').val();
        var prjctName = $('#Name').val();
        $.ajax({
            type: "get",
            url: "/Project/gotoCreateProjectTaskForm",
            data: { projectID: prjctId },
            traditional: true,
            success: function (data) {
                $("#dvNothing").html(data);
                $('#dvOverlay').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while open SR', XMLHttpRequest);
            }
        });
    });

    $('#imgAddTime').click(function () {
        var TaskId = $('#Id').val();
        var TaskName = $('#Name').val();
        $.ajax({
            type: "get",
            url: "/Task/gotoCreateTaskTimeForm",
            data: { task_id: TaskId },
            traditional: true,
            success: function (data) {
                $("#dvNothing").html(data);
                $('#TaskId').val(TaskId);
                $('#TaskName').val(TaskName);
                $('#dvOverlay').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while open SR', XMLHttpRequest);
            }
        });
    });

    $('#LogTime').click(function () {
        var TaskId = $('#Id').val();
        var TaskName = $('#Name').val();
        $.ajax({
            type: "get",
            url: "/Task/gotoCreateTaskTimeForm",
            data: { task_id: TaskId },
            traditional: true,
            success: function (data) {
                $("#dvNothing").html(data);
                $('#TaskId').val(TaskId);
                $('#TaskName').val(TaskName);
                //$('#dvOverlay').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while open SR', XMLHttpRequest);
            }
        });
    });

    $('#Resolve').click(function () {
        var TaskId = $('#Id').val();
        var TaskName = $('#Name').val();
        $.ajax({
            type: "get",
            url: "/Task/gotoResolveTaskForm",
            traditional: true,
            success: function (data) {
                $("#dvNothing").html(data);
                $('#TaskId').val(TaskId);
                $('#TaskName').val(TaskName);
                //$('#dvOverlay').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while open SR', XMLHttpRequest);
            }
        });
    });

    $('#imgAddComment').click(function () {
        var TaskId = $('#Id').val();
        var TaskName = $('#Name').val();
        $.ajax({
            type: "get",
            url: "/Task/gotoCreateTaskCommentForm",
            data: { task_id: TaskId },
            traditional: true,
            success: function (data) {
                $("#dvNothing").html(data);
                $('#TaskId').val(TaskId);
                $('#TaskName').val(TaskName);
                //$('#dvOverlay').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while open SR', XMLHttpRequest);
            }
        });
    });

    $('#dvEntityNavigation').click(function () {
        $('#dvOtherRecords').toggle('blind', 200);
        var dvHeight = $('#dvOtherRecords').height();
        var TaskId = $('#Id').val();
        if (dvHeight < 10) {

            $.ajax({
                type: "get",
                url: "/Task/OtherRecords",
                data: { task_id: TaskId },
                traditional: true,
                success: function (data) {
                    $("#dvNothing").html(data);
                    $("#tblOtherRecords tr").remove();
                    create_selection_table_with_no_cbox(data.success, "tblOtherRecords", "Tasks");
                    //var roleTable = $('#tblOtherRecords').dataTable({
                    //    "iDisplayLength": 20,
                    //    "aLengthMenu": [[10, 20, 50, -1], [10, 20, 50, "All"]],
                    //    "sPaginationType": "full_numbers"
                    //});
                    //roleTable.fnSort([[1, 'asc']]);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    error_handlerXML('Error occured while open SR', XMLHttpRequest);
                }
            });
        }
    });

    function DropDownTaskState(el) {
        this.dd = el;
        this.placeholder = this.dd.children('input');
        this.opts = this.dd.find('ul.dropdown > li');
        this.val = '';
        this.index = -1;
        this.initEvents();
    }

    DropDownTaskState.prototype = {
        initEvents: function () {
            var obj = this;

            obj.dd.on('click', function (event) {
                $(this).toggleClass('active');
                return false;
            });

            obj.opts.on('click', function () {
                var opt = $(this);
                obj.val = opt.text();
                obj.index = opt.index();
                obj.placeholder.text(obj.val);
                $('#TaskStateName').val(obj.val);
            });
        },
        getValue: function () {
            return this.val;
        },
        getIndex: function () {
            return this.index;
        }
    }

    function DropDownState(el) {
        this.dd = el;
        this.placeholder = this.dd.children('input');
        this.opts = this.dd.find('ul.dropdown > li');
        this.val = '';
        this.index = -1;
        this.initEvents();
    }
    DropDownState.prototype = {
        initEvents: function () {
            var obj = this;

            obj.dd.on('click', function (event) {
                $(this).toggleClass('active');
                return false;
            });

            obj.opts.on('click', function () {
                var opt = $(this);
                obj.val = opt.text();
                obj.index = opt.index();
                obj.placeholder.text(obj.val);
                $('#StateName').val(obj.val);
            });
        },
        getValue: function () {
            return this.val;
        },
        getIndex: function () {
            return this.index;
        }
    }

    $(function () {
        var dd = new DropDownState($('#dvState'));

        $(document).click(function () {
            // all dropdowns
            $('.wrapper-dropdown-3').removeClass('active');
        });

    });

    $(function () {
        var dd = new DropDownTaskState($('#dvTaskState'));

        $(document).click(function () {
            // all dropdowns
            $('.wrapper-dropdown-3').removeClass('active');
        });

    });

    $('ul#ulState > li').click(function () {
        var value = $(this).attr('id');
        $('#StateId').val(value);        
    });
    
    $('ul#ulTaskState > li').click(function () {
        var value = $(this).attr('id');
        $('#TaskStateId').val(value);
    });

    $('#btnOtherRecords').click(function () {
        //$('#btnOtherRecords tr').remove();
        //$('#btnOtherRecords th').remove();
        $('#tblOtherTasks').dataTable({
            "sScrollX": "100%",
            "bRetrieve": true,
            "sScrollXInner": "110%"
        });
        var options = { to: { width: 200, height: 60 } };
        $("#dvOtherRec").toggle(null, options, 700);
        var TaskId = $('#Id').val();
        $.ajax({
            type: "get",
            url: "/Task/getOtherTasks",
            data: { task_id: TaskId },
            success: function (data) {
                $("#tblOtherTasks tr").remove();
                document.getElementById('tblOtherTasks').innerHTML = "";
                create_selection_table_with_no_cbox(data.success, "tblOtherTasks", "Tasks");
                $('#tblOtherTasks').dataTable({
                    "sScrollX": "100%",
                    "bRetrieve": true,
                    "sScrollXInner": "110%"
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while getting other taks', XMLHttpRequest);
            }
        });
    });

    $('#imgTeamAddUser').click(function () {
        var TeamId = $('#Id').val();
        var TeamName = $('#Name').val();
        $.ajax({
            type: "get",
            url: "/Team/gotoTeamAddUser",
            data: { record_id: TeamId },
            traditional: true,
            success: function (data) {
                $("#dvNothing").html(data);
                //$('#TaskId').val(TaskId);
                //$('#TaskName').val(TaskName);
                //$('#dvOverlay').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while open SR', XMLHttpRequest);
            }
        });
    });


    $('#imgAddUsertoProject_user').click(function () {
        debugger;
        var UserId = $('#Id').val();
        var UserName = $('#LastName').val() + ", " + $('#FirstName').val();
        $('#EntityId').val(UserId);
        $('#UserName').val(UserName);
        $.ajax({

            type: "get",
            url: "/User/gotoManageProjectUser",
            success: function (data) {
                $("#dvNothing").html(data);
                $('#EntityId').val(UserId);
                $('#UserName').val(UserName);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while open SR', XMLHttpRequest);
            }
        });
    });
    $('#imgAddRole').click(function () {
        var UserId = $('#Id').val();
        var UserName = $('#LastName').val() + ", " + $('#FirstName').val();
        $.ajax({
            type: "get",
            url: "/User/gotoManageRole",
            success: function (data) {
                $("#dvNothing").html(data);
                $('#EntityId').val(UserId);
                $('#UserName').val(UserName);
                //$('#dvOverlay').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while open SR', XMLHttpRequest);
            }
        });
    });

    ///imgprojectAddUser
    $('#imgprojectAddUser').click(function () {
        var projectId = $('#Id').val();
        $.ajax({
            type: "get",
            url: "/Project/gotoProjectAddUser",
            success: function (data) {
                $("#dvNothing").html(data);
                $('#EntityId').val(projectId);
                //$('#dvOverlay').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error_handlerXML('Error occured while open SR', XMLHttpRequest);
            }
        });
    });

});
