function getParameterByName(name) {
	name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
	var regexS = "[\\?&]" + name + "=([^&#]*)";
	var regex = new RegExp(regexS);
	var results = regex.exec(window.location.search);
	if (results == null)
		return "";
	else
		return decodeURIComponent(results[1].replace(/\+/g, " "));
}

function refreshPage() {
	var style = getParameterByName("style");

	if (style != null && style >= 3) {
		style = 1;
	}
	else {
		style = style + 1;
	}

	window.location.href = window.location.href + "?style=" + style;
}

$(document).ready(function () {
	var style = getParameterByName("style");

	if (style != null && style.length > 0 && style < 4) {
		$('link#style').attr('href', '../Content/Lab6/Style' + style + '.css');	
	}
	else {
		$('link#style').attr('href', '../Content/Lab6/Style1.css');		
	}

	if ($("#RefreshInterval").length > 0) {
		var intervalValue = $('#RefreshIntervalValue').val();

		if (intervalValue.length > 0) {
			$("#RefreshInterval").val(intervalValue);
		}
		else {
			$("#RefreshInterval").val("1");
		}

		if (intervalValue != null && intervalValue.length > 0) {
			window.setTimeout('refreshPage()', intervalValue * 60 * 1000);
		}
	}

	$("#RefreshInterval").change(function () {
		$('#RefreshIntervalValue').val($("#RefreshInterval").val());
		var intervalValue = $('#RefreshIntervalValue').val();

		if (intervalValue != null && intervalValue.length > 0) {
			refreshPage();
		}
	});
});