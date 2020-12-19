$(document).ready(function () {
    loadData("", "");
    var FaId = "";
    var ComId = "";
    $("#FacultyId").change(function () {
        FaId = $("#FacultyId").val();
        $.ajax({
            url: '/Intern/GetCompanyById',
            type: 'POST',
            data: { ID: FaId },
            success: function (response) {
                var len = response.length;
                $("#companyId").empty();
                $("#companyId").append("<option value='" + "" + "'>" + "Chọn Công ty" + "</option>");
                for (var i = 0; i < len; i++) {
                    var id = response[i].Value;
                    var name = response[i].Text;
                    $("#companyId").append("<option value='" + id + "'>" + name + "</option>");
                }

            }
        });
        loadData(FaId, "");
    });

    $("#companyId").change(function () {
        console.log($("#companyId").val());
        loadData(FaId, $("#companyId").val());
    });

});

function loadData(facultyID, companyID) {
    $.ajax({
        url: '/Intern/LoadData',
        type: 'GET',
        data: {
            facultyID: facultyID,
            companyID: companyID,
        },
        dataType: 'json',
        success: function (response) {
            if (response.status) {
                var data = response.dataOne;
                var html = '';
                var template = $('#data-template').html();
                $('#myTable').empty();
                $.each(data, function (i, item) {
                    var date = new Date(parseInt(item.Birthday.substr(6, item.Birthday.length - 8)));
                    var dateone = date.toLocaleDateString();
                    html += Mustache.render(template, {
                        PersonID: item.PersonID,
                        StudentCode: item.StudentCode,
                        FullName: item.FullName,
                        Birthday: dateone,
                        Company: item.NameOfCompany,
                        InternShip: item.NameOfInternship,
                        Result: item.Result,
                    });

                });
                $('#myTable').html(html);
                var data1 = response.dataTwo;
                var html1 = '';
                var template1 = $('#data-template1').html();
                $('#myTable1').empty();
                $.each(data1, function (i, item1) {
                    var date = new Date(parseInt(item1.Birthday.substr(6, item1.Birthday.length - 8)));
                    var datetwo = date.toLocaleDateString();
                    html1 += Mustache.render(template1, {
                        PersonID: item1.PersonID,
                        StudentCode: item1.StudentCode,
                        FullName: item1.FullName,
                        Birthday: datetwo,
                        Company: item1.NameOfCompany,
                    });

                });
                $('#myTable1').html(html1);
            }
        }
    })
}