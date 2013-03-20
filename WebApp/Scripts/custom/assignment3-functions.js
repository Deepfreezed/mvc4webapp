$(document).ready(function () {
	var oTable;

	var delay = (function () {
		var timer = 0;
		return function (callback, ms) {
			clearTimeout(timer);
			timer = setTimeout(callback, ms);
		};
	})();

	if ($("#tbl_airports").length > 0) {
		oTable = $("#tbl_airports").dataTable({
			"bPaginate": true,
			"bLengthChange": false,
			"bFilter": true,
			"bSort": false,
			"bInfo": false,
			"sPaginationType": "full_numbers",
			"iDisplayLength": 25,
			"sDom": '<"top"iflp<"clear">>rt<"bottom"iflp<"clear">>'
		});
	}

	if (oTable != null) {
		$(".dataTables_filter").hide(); 

		var filter = $("#AirportLocation").val();
		if (filter != null && filter.length > 0) {
			oTable.fnFilter("^" + filter, 2, true);
		}
	}

	$("#AirportLocation").keyup(function () {
		delay(function () {
			var filter = $("#AirportLocation").val();
			oTable.fnFilter("^" + filter, 2, true);
		}, 100);
	});
});