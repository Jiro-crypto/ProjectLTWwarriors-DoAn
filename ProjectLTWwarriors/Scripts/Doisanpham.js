document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.prev-arrow, .next-arrow').forEach(button => {
        button.addEventListener('click', function () {
            const category = this.getAttribute('data-category');
            const groups = document.querySelectorAll(`#DanhSach_${category} .Danh_Sach_San_Pham_Group`);
            let currentGroupIndex = Array.from(groups).findIndex(group => group.style.display === 'flex');

            // Vòng quay qua các nhóm sản phẩm
            currentGroupIndex = (currentGroupIndex + (this.classList.contains('next-arrow') ? 1 : -1) + groups.length) % groups.length;

            // Ẩn nhóm hiện tại và hiển thị nhóm tiếp theo
            groups.forEach((group, index) => group.style.display = (index === currentGroupIndex) ? 'flex' : 'none');
        });
    });
});