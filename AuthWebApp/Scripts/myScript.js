const allCheckbox = document.querySelector('#allCheckbox');
const userCheckboxes = document.querySelectorAll('.userCheckbox');
allCheckbox.onclick = function () {
    let check = this.checked;
    for (let i = 0; i < userCheckboxes.length; i++) {
        userCheckboxes[i].checked = check;
    }
}
