$(document).ready(function () {
    var schoolId = "";
    $("#SchoolId").change(function () {
        schoolId = $("#SchoolId").val();
        $.ajax({
            url: '/Intern/GetFacultyById',
            type: 'POST',
            data: { ID: schoolId },
            success: function (response) {
                var len = response.length;
                $("#FacultyId").empty();
                $("#FacultyId").append("<option value='" + "" + "'>" + "--Select Faculty--" + "</option>");
                for (var i = 0; i < len; i++) {
                    var id = response[i].Value;
                    var name = response[i].Text;
                    $("#FacultyId").append("<option value='" + id + "'>" + name + "</option>");
                }

            }
        });
    });


});

