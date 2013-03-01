$(document).ready(function () {
	if ($('#growlUI').length) {
		$.blockUI({
			message: $('div.growlUI'),
			fadeIn: 400,
			fadeOut: 400,
			timeout: 1000,
			showOverlay: false,
			centerX: true,
			centerY: true,
			css: {
				width: '600px',
				height: '100px',
				top: '20%',
				left: '40%',
				right: '',
				border: 'none',
				padding: '5px',
				backgroundColor: '#000',
				'-webkit-border-radius': '10px',
				'-moz-border-radius': '10px',
				opacity: .6,
				color: '#fff'
			}
		});
	}
});