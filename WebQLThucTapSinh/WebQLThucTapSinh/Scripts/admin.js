
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

});