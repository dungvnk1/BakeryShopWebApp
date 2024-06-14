function showCP() {
    document.getElementById('cpbox').style.display = 'block';

}

$(document).ready(function () {
    console.log("ready");
    $(".appv").click(function () {
        var code = $("#ipv").val();
        console.log(code);

        $.ajax({
            url: "Checkout/UseVoucher",
            data: { code: code },
            success: function (data) {
                console.log("ajax ok");
                if (!data.exist) {
                    Swal.fire("Mã giảm giá không tồn tại!");
                    $("#ipv").val("");
                    console.log("not exist");
                }
                else {
                    if (data.inuse) {
                        Swal.fire("Bạn đã sử dụng mã giảm giá này trước đây!");
                        $("#ipv").val("");
                        console.log("inuse");
                    }
                    else {
                        if (!data.remain) {
                            Swal.fire("Mã giảm giá đã hết hạn!");
                            $("#ipv").val("");
                            console.log("not remain");
                        }
                        else {
                            console.log("good");
                            Swal.fire("Áp mã thành công!");
                            var dis = data.percent;
                            var reduce = stotal * dis / 100;
                            var sutotal = stotal - reduce;
                            var subtotal = Intl.NumberFormat('en-US').format(sutotal);
                            var red = Intl.NumberFormat('en-US').format(reduce);
                            $("#subtotal").text(subtotal + "đ");
                            $("#delv").css("display", "inline-block");
                            $("#codev").text("- (" + dis + "%) " + red + "đ");
                            $("#vch").css("display", "block");
                        }
                    }
                }
            },
            error: function () {

                Swal.fire({
                    icon: "error",
                    title: "Ôi...",
                    text: "Đã có lỗi xảy ra!"
                });
            }

        });
    });

    $("#delv").click(function () {
        var subtotal = Intl.NumberFormat('en-US').format(stotal);

        $("#ipv").val("");
        $(this).css("display", "none");
        $("#vch").css("display", "none");
        $("#subtotal").text(subtotal + "đ");
        Swal.fire("Đã huỷ áp dụng mã giảm giá!");
    });

    $(".order").click(function () {
        var address = $(".adr").val();
        var phone = $(".pho").val();
        var note = $(".note").val();
        var voucher = $("#ipv").val();
        console.log(address); console.log(phone); console.log(note); console.log(voucher);

        $.ajax({
            type: "POST",
            url: "Checkout/CreateBill",
            data: {
                address,
                number: phone,
                note,
                voucher
            },
            success: function () {
                Swal.fire({
                    title: "Thành công!",
                    text: "Bạn đã đặt hàng thành công. Hãy theo dõi trạng thái đơn hàng, đơn hàng sẽ sớm được giao đến bạn.",
                    icon: "success",
                    showDenyButton: true,
                    confirmButtonColor: "#3085d6",
                    denyButtonColor: "#d33",
                    denyButtonText: "Lịch sử đặt hàng",
                    confirmButtonText: "Về Trang chủ"
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.href = "/";
                    }
                    else if (result.isDenied) {
                        window.location.href = "/Bills/Index";
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
    });
});

