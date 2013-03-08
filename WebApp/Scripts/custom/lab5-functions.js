function submitPage() {
	$('#Lab5Form').submit();
}

$(document).ready(function () {
	if ($("#RefreshInterval").length > 0) {
		var intervalValue = $('#RefreshIntervalValue').val();

		if (intervalValue.length > 0) {
			$("#RefreshInterval").val(intervalValue);
		}
		else {
			$("#RefreshInterval").val("1");
		}

		if (intervalValue != null && intervalValue.length > 0) {
			window.setTimeout('submitPage()', intervalValue * 60 * 1000);
		}
	}

	$("#RefreshInterval").change(function () {
		$('#RefreshIntervalValue').val($("#RefreshInterval").val());
		var intervalValue = $('#RefreshIntervalValue').val();

		if (intervalValue != null && intervalValue.length > 0) {
			submitPage();
		}
	});
});