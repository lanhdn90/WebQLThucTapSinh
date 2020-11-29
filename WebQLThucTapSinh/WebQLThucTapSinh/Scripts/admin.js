
$(document).ready(function(){
    CKEDITOR.replace("FullDescription");

    $(function () {
        $('.datepicker').datepicker({
            dateFormat: "dd-mm-yy",
            changeMonth: true,
            changeYear: true,
            yearRange: '1970:2030',
        });
    });

    $("#SelectImage").click(function () {
        var finder = new CKFinder();
        finder.selectActionFunction = function (fileUrl) {
            $("#Image").val(fileUrl);
        };
        finder.popup();
    });

    $("#SelectLogo").click(function () {
        var finder = new CKFinder();
        finder.selectActionFunction = function (fileUrl) {
            $("#Logo").val(fileUrl);
        };
        finder.popup();
    });

    $(".deleteQuestion").off('click').on('click', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var name = $(this).data('name');
        var r = confirm('Bạn có muốn xóa câu hỏi ' + name + ' hay không ?');
        if (r == true) {
            $.ajax({
                url: '/Question/Delete',
                data: { id: id },
                type: 'POST',
                success: function (data) {
                    var json = JSON.parse(data);
                    alert(json);
                    window.location.reload();
                },
                error: function (err) {
                    alert("Đã xảy ra lỗi" + err.responseText);
                }
            });
        }
    });

    $('.btn-task').click(function (e) {
        // Khai báo tham số
        e.preventDefault();
        var checkbox = document.getElementsByName('dschon');
        var listTask = [];
        var count = 0;
        var id = $(this).data('id');
        // Lặp qua từng checkbox để lấy giá trị
        for (var i = 0; i < checkbox.length; i++) {
            if (checkbox[i].checked === true) {

                listTask[count] = checkbox[i].value;
                count++;
            }
        }
        $.ajax({
            url: '/Tasks/AddTask',
            traditional: true,
            data: {
                listTask: listTask,
                id: id,
            },
            type: "GET",
            success: function (response) {
                if (response == 'false') {
                    alert("Thêm Bài học thất bại");
                } else {
                    alert("Thêm Bài học thành công");
                }
            }
        });
    });

    $(".deleteTask").off('click').on('click', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var name = $(this).data('name');
        var r = confirm('Bạn có muốn xóa Bài học ' + name + ' hay không ?');
        if (r == true) {
            $.ajax({
                url: '/Tasks/Delete',
                data: { id: id },
                type: 'POST',
                success: function (data) {
                    var json = JSON.parse(data);
                    alert(json);
                    window.location.reload();
                },
                error: function (err) {
                    alert("Đã xảy ra lỗi" + err.responseText);
                }
            });
        }
    });

    $(".deleteTaskInInternShip").off('click').on('click', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var name = $(this).data('name');
        var r = confirm('Bạn có chắc muốn xóa Bài học ' + name + ' hay không ?');
        if (r == true) {
            $.ajax({
                url: '/InternShip/DeleteInternShipWithTask',
                data: { id: id },
                type: 'POST',
                success: function (data) {
                    var json = JSON.parse(data);
                    alert(json);
                    window.location.reload();
                },
                error: function (err) {
                    alert("Đã xảy ra lỗi" + err.responseText);
                }
            });
        }
    });

    $(".deleteInternShip").off('click').on('click', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var name = $(this).data('name');
        var r = confirm('Bạn có chắc muốn xóa Khóa học ' + name + ' hay không ?');
        if (r == true) {
            $.ajax({
                url: '/InternShip/Delete',
                data: { id: id },
                type: 'POST',
                success: function (data) {
                    var json = JSON.parse(data);
                    alert(json);
                    window.location.reload();
                },
                error: function (err) {
                    alert("Đã xảy ra lỗi" + err.responseText);
                }
            });
        }
    });

    var id;
    $('.idSort').mouseover(function () {
        //var sort = $('#sort option:selected').text();;
        var task = $(this).data('ids');
        id = parseInt(task);
    });

    $('.dsach').change(function () {
        var sort = parseInt($(this).val());
        $.ajax({
            url: '/InternShip/UpdateSort',
            data: {
                id: id,
                sort: sort },
            type: 'POST',
            success: function (data) {
                var json = JSON.parse(data);
                alert(json);
                window.location.reload();
            },
            error: function (err) {
                alert("Đã xảy ra lỗi" + err.responseText);
            }
        });
        //alert(sort + "id" + id);
    });

    $('.btn-active').off('click').on('click', function (e) {
        e.preventDefault();
        var btn = $(this)
        var id = btn.data('id');
        $.ajax({
            url: '/InternShip/ChangeStatus',
            data: { id: id },
            dataType: "json",
            type: "POST",
            success: function (response) {
                if (response.status == true) {
                    btn.text('Kích hoạt');
                } else {
                    btn.text('Khóa');
                }
                window.location.reload();
            }
        });
    });

    $('.btn-active1').off('click').on('click', function (e) {
        e.preventDefault();
        var btn = $(this)
        var id = btn.data('id');
        $.ajax({
            url: '/Leader/ChangeStatus',
            data: { id: id },
            dataType: "json",
            type: "POST",
            success: function (response) {
                if (response.status == true) {
                    btn.text('Kích hoạt');
                } else {
                    btn.text('Khóa');
                }
                window.location.reload();
            }
        });
    });

    $('.btn-active2').off('click').on('click', function (e) {
        e.preventDefault();
        var btn = $(this)
        var id = btn.data('id');
        $.ajax({
            url: '/Representative/ChangeStatus',
            data: { id: id },
            dataType: "json",
            type: "POST",
            success: function (response) {
                if (response.status == true) {
                    btn.text('Kích hoạt');
                } else {
                    btn.text('Khóa');
                }
                window.location.reload();
            }
        });
    });

    $(".deleteRepresentative").off('click').on('click', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var name = $(this).data('name');
        var r = confirm('Bạn có chắc muốn xóa giáo vụ ' + name + ' hay không ?');
        if (r == true) {
            $.ajax({
                url: '/Representative/Delete',
                data: { id: id },
                type: 'POST',
                success: function (data) {
                    var json = JSON.parse(data);
                    alert(json);
                    window.location.reload();
                },
                error: function (err) {
                    alert("Đã xảy ra lỗi" + err.responseText);
                }
            });
        }
    });

    $('.btn-active3').off('click').on('click', function (e) {
        e.preventDefault();
        var btn = $(this)
        var id = btn.data('id');
        var name = $(this).data('name');
        $.ajax({
            url: '/Faculty/ChangeStatus',
            data: { id: id },
            dataType: "json",
            type: "POST",
            success: function (response) {
                if (response.status == true) {
                    btn.text('Kích hoạt');
                    alert('Chú ý: Bạn cần kích hoạt (hoặc thêm mới) lại giáo vụ của ' + name);
                } else {
                    btn.text('Khóa');
                }
                window.location.reload();
            }
        });
    });

    $(".deleteFaculty").off('click').on('click', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var name = $(this).data('name');
        var r = confirm('Bạn có chắc muốn xóa ' + name + ' hay không ?');
        if (r == true) {
            $.ajax({
                url: '/Faculty/Delete',
                data: { id: id },
                type: 'POST',
                success: function (data) {
                    var json = JSON.parse(data);
                    alert(json);
                    window.location.reload();
                },
                error: function (err) {
                    alert("Đã xảy ra lỗi" + err.responseText);
                }
            });
        }
    });


    var task;
    $('.btn-active4').off('click').on('click', function (e) {
        e.preventDefault();
        var btn = $(this)
        var id = btn.data('id');
        $.ajax({
            url: '/Company/ChangeStatus',
            data: { id: id },
            dataType: "json",
            type: "POST",
            success: function (response) {
                if (response.status == true) {
                    btn.text('Kích hoạt');
                } else {
                    btn.text('Khóa');
                }
            }
        });
    });

    $('.idSort1').mouseover(function () {
        task = $(this).data('ids');
    });

    $('.gender').change(function () {
        var valu = $(this).val();
        $.ajax({
            type: 'GET',
            data: {
                id: task,
                val: valu,
            },
            url: '/Company/extension',
            success: function (result) {
                if (result == 'True') {
                    window.location.href = '/Company/Index/';
                } else {
                    alert("Gia hạn thất bại");
                }
            }
        });
    });

    $(".deleteCompany").off('click').on('click', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var name = $(this).data('name');
        var r = confirm('Bạn có chắc muốn xóa công ty ' + name + ' hay không ?');
        if (r == true) {
            $.ajax({
                url: '/Company/Delete',
                data: { id: id },
                type: 'POST',
                success: function (data) {
                    var json = JSON.parse(data);
                    alert(json);
                    window.location.reload();
                },
                error: function (err) {
                    alert("Đã xảy ra lỗi" + err.responseText);
                }
            });
        }
    });

    $('.btn-active5').off('click').on('click', function (e) {
        e.preventDefault();
        var btn = $(this)
        var id = btn.data('id');
        $.ajax({
            url: '/School/ChangeStatus',
            data: { id: id },
            dataType: "json",
            type: "POST",
            success: function (response) {
                if (response.status == true) {
                    btn.text('Kích hoạt');
                } else {
                    btn.text('Khóa');
                }
            }
        });
    });

    $('.idSort2').mouseover(function () {
        task = $(this).data('ids');
    });

    $('.gender-1').change(function () {
        var valu = $(this).val();
        $.ajax({
            type: 'GET',
            data: {
                id: task,
                val: valu,
            },
            url: '/School/extension',
            success: function (result) {
                if (result == 'True') {
                    window.location.href = '/School/Index/';
                } else {
                    alert("Gia hạn thất bại");
                }
            }
        });
    });

    $(".deleteSchool").off('click').on('click', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var name = $(this).data('name');
        var r = confirm('Bạn có chắc muốn xóa trườngn ' + name + ' hay không ?');
        if (r == true) {
            $.ajax({
                url: '/School/Delete',
                data: { id: id },
                type: 'POST',
                success: function (data) {
                    var json = JSON.parse(data);
                    alert(json);
                    window.location.reload();
                },
                error: function (err) {
                    alert("Đã xảy ra lỗi" + err.responseText);
                }
            });
        }
    });

});