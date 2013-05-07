/* Table initialisation */
$(document).ready(function () {
	var oTable;

	oTable = $('#mortgage_listing').dataTable({
		"bPaginate": true,
		"bLengthChange": false,
		"bFilter": false,
		"bSort": true,
		"bInfo": false,
		"iDisplayLength": 5,
		"sDom": "<'row'<'span6'l><'span6'f>r>t<'row'<'span6'i><'span6'p>><'top'i>rt<'bottom'lp><'clear'>",
		"sPaginationType": "bootstrap",
		"oLanguage": {
			"sLengthMenu": "_MENU_ records per page"
		},
		"fnDrawCallback": function (oSettings) {
			if (oSettings.aiDisplay.length == 0) {
				return;
			}

			var nTrs = $('#mortgage_listing tbody tr');
			var iColspan = nTrs[0].getElementsByTagName('td').length;
			var sLastGroup = "";			
		},
		"aaSortingFixed": [[0, 'asc']],
		"aaSorting": [[1, 'asc']]
	});
});

$(document).ready(function () {
	var oTable;

	oTable = $('#mortgage_amortization').dataTable({
		"bPaginate": true,
		"bLengthChange": false,
		"bFilter": false,
		"bSort": true,
		"bInfo": false,
		"iDisplayLength": 15,
		"sDom": "<'row'<'span6'l><'span6'f>r>t<'row'<'span6'i><'span6'p>><'top'i>rt<'bottom'lp><'clear'>",
		"sPaginationType": "bootstrap",
		"oLanguage": {
			"sLengthMenu": "_MENU_ records per page"
		},
		"fnDrawCallback": function (oSettings) {
			if (oSettings.aiDisplay.length == 0) {
				return;
			}

			var nTrs = $('#mortgage_amortization tbody tr');
			var iColspan = nTrs[0].getElementsByTagName('td').length;
			var sLastGroup = "";
		},
		"aaSortingFixed": [[0, 'asc']],
		"aaSorting": [[1, 'asc']]
	});
});