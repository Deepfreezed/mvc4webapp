/* Table initialisation */
$(document).ready(function () {
	var oTable;

	oTable = $('#course_listing').dataTable({
		"bPaginate": true,
		"bLengthChange": false,
		"bFilter": true,
		"bSort": true,
		"bInfo": false,
		"iDisplayLength": 10,
		"sDom": "<'row'<'span6'l><'span6'f>r>t<'row'<'span6'i><'span6'p>><'top'i>rt<'bottom'lp><'clear'>",
		"sPaginationType": "bootstrap",
		"oLanguage": {
			"sLengthMenu": "_MENU_ records per page"
		}
	});
});