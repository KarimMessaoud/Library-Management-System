function confirmDelete(uniqueId, isDeleteClicked) {
    var deleteSpanId = 'deleteSpan_' + uniqueId;
    var confirmDeleteSpanId = 'confirmDeleteSpan_' + uniqueId;

    if (isDeleteClicked) {
        $('#' + deleteSpanId).hide();
        $('#' + confirmDeleteSpanId).show();
    }
    else {
        $('#' + deleteSpanId).show();
        $('#' + confirmDeleteSpanId).hide();
    }
}