window.showSweetAlert = (type, title, text) => {
    Swal.fire({
        icon: type,
        title: title,
        text: text,
    });
};