//$(document).ready(function () {
$(".canc").click(function () {
    console.log("canc");
    var id = $(this).data("id");

    Swal.fire({
        title: "Nhập lí do",
        input: "text",
        inputLabel: "Lí do từ chối đơn hàng",
        showCancelButton: true,
        inputValidator: (value) => {
            if (!value) {
                return "Không được để trống!";
            }
            else {
                $.ajax({
                    url: "Orders/Cancel",
                    data: {
                        id: id,
                        reason: value
                    },
                    success: function () {
                        var cancelSelector = "#stt_" + id;
                        var cancSelector = "#canc_" + id;
                        console.log(cancelSelector);
                        $(cancelSelector).text("Bị từ chối");
                        $(cancSelector).css("display", "none");

                        Swal.fire({
                            title: "Thành công!",
                            text: "Đã từ chối đơn hàng.",
                            confirmButtonText: "Đồng ý"
                        }).then((result) => {
                            if (result.isConfirmed) {
                                window.location.reload();
                            }
                        });
                    },
                    error: function () {
                        Swal.fire({
                            icon: "error",
                            title: "Ôi...",
                            text: "Đã có lỗi xảy ra!"
                        });
                    }
                });
            }
        }
    });

});

$(".reason").click(function () {
    console.log("rs");
    var id = $(this).data("id");
    $.ajax({
        url: "Bills/GetCancelReason",
        data: { id: id },
        success: function (data) {
            Swal.fire({
                title: "Lí do từ chối đơn hàng",
                text: data.reason,
                confirmButtonText: "Đồng ý"
            });
        },
        error: function () {
            Swal.fire({
                icon: "error",
                title: "Ôi...",
                text: "Đã có lỗi xảy ra!"
            });
        }
    })
});
//});