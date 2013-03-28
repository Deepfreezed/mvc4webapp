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

function updateURLParameter(url, param, paramVal) {
	var TheAnchor = null;
	var newAdditionalURL = "";
	var tempArray = url.split("?");
	var baseURL = tempArray[0];
	var additionalURL = tempArray[1];
	var temp = "";

	if (additionalURL) {
		var tmpAnchor = additionalURL.split("#");
		var TheParams = tmpAnchor[0];
		TheAnchor = tmpAnchor[1];
		if (TheAnchor)
			additionalURL = TheParams;

		tempArray = additionalURL.split("&");

		for (i = 0; i < tempArray.length; i++) {
			if (tempArray[i].split('=')[0] != param) {
				newAdditionalURL += temp + tempArray[i];
				temp = "&";
			}
		}
	}
	else {
		var tmpAnchor = baseURL.split("#");
		var TheParams = tmpAnchor[0];
		TheAnchor = tmpAnchor[1];

		if (TheParams)
			baseURL = TheParams;
	}

	if (TheAnchor)
		paramVal += "#" + TheAnchor;

	var rows_txt = temp + "" + param + "=" + paramVal;
	return baseURL + "?" + newAdditionalURL + rows_txt;
}

function refreshPage() {
	var style = parseInt(getParameterByName("style"));
	var refresh = parseInt(getParameterByName("refresh"));
		
	if (style != null && style >= 3) {
		style = 1;
	}
	else if (style != null && (style < 3 && style > 0)) {
		style = style + 1;
	}
	else {
		style = 1;
	}

	var newUrl = updateURLParameter(window.location.href, "style", style);

	var intervalValue = $('#RefreshIntervalValue').val();
	if (intervalValue != null && intervalValue.length > 0) {
		newUrl = updateURLParameter(newUrl, "refresh", intervalValue);
	}	

	window.location.href = newUrl;
}

$(document).ready(function () {
	var style = getParameterByName("style");
	var refresh = parseInt(getParameterByName("refresh"));

	if (style != null && style.length > 0 && style < 4) {
		$('link#style').attr('href', '../Content/Lab6/Style' + style + '.css');
	}
	else {
		$('link#style').attr('href', '../Content/Lab6/Style1.css');
	}

	if ($("#RefreshInterval").length > 0) {
		var intervalValue = parseInt(getParameterByName("refresh"));
		
		if (intervalValue > 0) {
			$("#RefreshInterval").val(intervalValue);
		}
		else {
			$("#RefreshInterval").val("1");
		}

		if (intervalValue != null && intervalValue.length > 0) {
			window.setTimeout('refreshPage()', intervalValue * 60 * 100);
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