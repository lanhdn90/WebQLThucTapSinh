
$(document).ready(function(){
    CKEDITOR.replace("Note");

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
});