$.validator.addMethod(
    'notequal',
   function (value, element, params) {
        if (!this.optional(element)) {
            var other = $('#' + params.other) // this equals to #User_02_Id
            return (other.val() != value);
        }
        return true;
    });

$.validator.unobtrusive.adapters.add(
    'notequal',
    ['other'],
    function (options) {
        var params = {
            other: options.params.other,
        };
        options.rules['notequal'] = params;
        options.messages['notequal'] = options.message;
    });