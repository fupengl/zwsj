// Initialize ajax autocomplete:
$('#searchKeyword').autocomplete({
    serviceUrl: 'searchByKeWord',
    onSelect: function (suggestion) {
        alert('You selected: ' + suggestion.value + ', ' + suggestion.data);
    }
});