﻿//Increase button
$(document).ready(function () {
    $(".increment").click(function () {
        var targetInputSelector = $(this).data("target");
        var totalSelector = "#total_" + $(this).data("id");

        $.ajax({
            url: "OrderItems/IncOne",
            data: {
                id: $(this).data("id")
            },
            success: function (response) {
                console.log(response);
                $(targetInputSelector).val(response.quantity);
                var total = Intl.NumberFormat('en-US').format(response.price);
                $(totalSelector).text(total + "đ");
                var subtotal = Intl.NumberFormat('en-US').format(response.total);
                $("#subtotal").text(subtotal + "đ");

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

//Decrease button
$(document).ready(function () {
    $(".decrement").click(function () {
        var targetInputSelector = $(this).data("target");
        var totalSelector = "#total_" + $(this).data("id");

        $.ajax({
            url: "OrderItems/DecOne",
            data: {
                id: $(this).data("id")
            },
            success: function (response) {
                console.log(response);
                $(targetInputSelector).val(response.quantity);
                var total = Intl.NumberFormat('en-US').format(response.price);
                $(totalSelector).text(total + "đ");
                var subtotal = Intl.NumberFormat('en-US').format(response.total);
                $("#subtotal").text(subtotal + "đ");

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

//Change input value
function check(id) {
    var inputSelector = "#quantity_" + id;
    var quantity = $(inputSelector).val();
    var maxQuantity = $(inputSelector).data("max");

    quan = parseInt(quantity);

    quan = isNaN(quan) ? 1 : quan;
    quan = (quan <= 0) ? 1 : quan;

    if (quan > maxQuantity) {
        Swal.fire({
            title: "Thông báo",
            text: "Rất tiếc, bạn chỉ có thể mua tối đa " + maxQuantity + " chiếc của loại bánh này.",
            confirmButtonText: "Đồng ý"
        });
    }

    var totalSelector = "#total_" + id;

    $.ajax({
        url: "OrderItems/UpdateQuantity",
        data: {
            id: id,
            quantity: quan
        },
        success: function (response) {
            $(inputSelector).val(response.quantity);
            var total = Intl.NumberFormat('en-US').format(response.price);
            $(totalSelector).text(total + "đ");
            var subtotal = Intl.NumberFormat('en-US').format(response.total);
            $("#subtotal").text(subtotal + "đ");
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

$(document).ready(function () {
    $(".del_cart").click(function () {
        var id = $(this).data("id");

        Swal.fire({
            title: "Bạn có chắc chắn muốn xoá?",
            text: "Bạn sẽ không thể hoàn tác!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            cancelButtonText: "Huỷ",
            confirmButtonText: "Xoá"
        }).then((result) => {
            if (result.isConfirmed) {

                $.ajax({
                    url: "OrderItems/DeleteItem",
                    data: { id: $(this).data("id") },
                    success: function (data) {
                        $("#item_" + id).css("display", "none");
                        var subtotal = Intl.NumberFormat('en-US').format(data.total);
                        $("#subtotal").text(subtotal + "đ");
                        var cq = $("#c_quantity").text();
                        var cqtt = parseInt(cq);
                        cqtt--;
                        if (cqtt > 0) {
                            $("#c_quantity").text(cqtt);
                        }
                        else {
                            $("#c_quantity").css("display", "none");
                        }
                        Swal.fire({
                            title: "Thành công!",
                            text: "Sản phẩm đã được xoá khỏi đơn hàng.",
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
        });
    });
});