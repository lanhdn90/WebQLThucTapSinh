$(document).ready(function () {
    var internShip = mySessionVariable;
    loadData(internShip);

    $('.btn-internship').click(function (e) {
        e.preventDefault();
        var id = parseInt($(this).data('id'));
        var name = $(this).data('name');
        $('#h3-heading').text(name);
        loadData(id);
    });


    $('.btn-sendinternship').click(function (e) {
        // Khai báo tham số
        e.preventDefault();
        var checkbox = document.getElementsByName('dschon1');
        var listIntern = [];
        var count = 0;
        var id = $(this).data('id');
        var name = $(this).data('name');
        // Lặp qua từng checkbox để lấy giá trị
        for (var i = 0; i < checkbox.length; i++) {
            if (checkbox[i].checked === true) {

                listIntern[count] = checkbox[i].value;
                count++;
            }
        }
        var r = confirm('Bạn có chắc muốn thêm thực tập sinh vào khóa học ' + name + '  này hay không ?');
        if (r == true) {
            $.ajax({
                url: '/Intern/AddIntern',
                traditional: true,
                data: {
                    listIntern: listIntern,
                    id: id,
                },
                type: "GET",
                success: function (response) {
                    if (response == 'false') {
                        alert("Thêm Thực tập sinh vào khóa học thất bại");
                    } else {
                        alert("Thêm Thực tập sinh vào khóa học thành công");
                        window.location.reload();
                    }
                }
            });
        }
        
    });
});
function loadData(internShipId) {
    $.ajax({
        url: '/Intern/LoadDataOfCompany',
        type: 'GET',
        data: {
            internShipId: internShipId,
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
                        FullName: item.FullName,
                        Birthday: dateone,
                        School: item.NameOfSchool,
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
                        FullName: item1.FullName,
                        Birthday: datetwo,
                        School: item1.NameOfSchool,
                    });

                });
                $('#myTable1').html(html1);
            }
        }
    })
}