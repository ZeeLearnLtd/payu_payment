var app = angular.module('PaymentLinkApp', []);

// ═══════════════════════════════════════════════════════════════════════════════
//  CONFIGURATION — Change when moving to production
// ═══════════════════════════════════════════════════════════════════════════════
var CONFIG = {
    EASEBUZZ_API_URL: 'https://kubapi.zeelearn.com/V1/easebuzzapi/api/payment/CreatePaymentLink',
    //EASEBUZZ_API_URL: 'http://localhost:3002/api/payment/CreatePaymentLink',
    EASEBUZZ_TOKEN: 'PGK-a7B9x2Qm8dR4sW1n'
};

function GetParameterValues(param) {
    var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}

function getDateString(daysFromNow) {
    var d = new Date();
    d.setDate(d.getDate() + (daysFromNow || 0));
    var dd = String(d.getDate()).padStart(2, '0');
    var mm = String(d.getMonth() + 1).padStart(2, '0');
    return dd + '-' + mm + '-' + d.getFullYear();
}


// ═══════════════════════════════════════════════════════════════════════════════
//  FIELD-LABEL & ERROR MAPPING HELPERS
// ═══════════════════════════════════════════════════════════════════════════════

var EASEBUZZ_FIELD_MAP = {
    'name': 'customer_name',
    'customer_name': 'customer_name',
    'email': 'customer_email',
    'customer_email': 'customer_email',
    'phone': 'customer_mobile',
    'mobile': 'customer_mobile',
    'customer_mobile': 'customer_mobile',
    'amount': 'indent_amount',
    'indent_amount': 'indent_amount',
    'message': 'ebMessage',
    'expiry_date': 'ebExpiryDate',
    'merchant_txn': 'merchant_txn',
    'udf1': 'paymentType',
    'udf3': 'location',
    'udf4': 'state'
};

var FIELD_LABELS = {
    'customer_name': 'Customer Name',
    'customer_email': 'Email',
    'customer_mobile': 'Mobile Number',
    'indent_amount': 'Amount',
    'state': 'State',
    'location': 'City / Location',
    'paymentType': 'Nature of Payment',
    'ebMessage': 'Message to Customer',
    'ebExpiryDate': 'Expiry Date',
    'merchant_txn': 'Transaction ID',
    'remarks': 'Remarks'
};

function mapEasebuzzField(apiField) {
    return EASEBUZZ_FIELD_MAP[(apiField || '').toLowerCase()] || apiField;
}

function getFieldLabel(formField) {
    return FIELD_LABELS[formField] || formField;
}


// ═══════════════════════════════════════════════════════════════════════════════
//  ROBUST LINK EXTRACTION — tries every possible response path
// ═══════════════════════════════════════════════════════════════════════════════

function extractPaymentLink(ebResp) {
    try {
        // Path 1: ebResp.data.payment_link (most common)
        if (ebResp.data && ebResp.data.payment_link) return ebResp.data.payment_link;

        // Path 2: ebResp.data.easebuzz_response.payment_url
        if (ebResp.data && ebResp.data.easebuzz_response) {
            var ebr = ebResp.data.easebuzz_response;
            if (ebr.payment_url) return ebr.payment_url;
            if (ebr.short_url) return ebr.short_url;
            if (ebr.payment_link) return ebr.payment_link;
        }

        // Path 3: ebResp.payment_link (flat / unwrapped response)
        if (ebResp.payment_link) return ebResp.payment_link;
        if (ebResp.payment_url) return ebResp.payment_url;
        if (ebResp.short_url) return ebResp.short_url;

        // Path 4: ebResp.easebuzz_response (without .data wrapper)
        if (ebResp.easebuzz_response) {
            var ebr2 = ebResp.easebuzz_response;
            if (ebr2.payment_url) return ebr2.payment_url;
            if (ebr2.short_url) return ebr2.short_url;
            if (ebr2.payment_link) return ebr2.payment_link;
        }

        // Path 5: ebResp.data is a string URL itself
        if (ebResp.data && typeof ebResp.data === 'string' && ebResp.data.indexOf('http') === 0) {
            return ebResp.data;
        }

        // Path 6: deep search — find first URL-like value anywhere in the response
        var found = deepFindUrl(ebResp, 0);
        if (found) return found;

    } catch (e) {
        console.error('[PaymentLink] Error extracting link from response:', e);
    }
    return '';
}

function deepFindUrl(obj, depth) {
    if (depth > 3 || !obj || typeof obj !== 'object') return '';
    var urlKeys = ['payment_link', 'payment_url', 'short_url', 'link', 'url'];
    for (var i = 0; i < urlKeys.length; i++) {
        var val = obj[urlKeys[i]];
        if (val && typeof val === 'string' && val.indexOf('http') === 0) {
            return val;
        }
    }
    for (var key in obj) {
        if (obj.hasOwnProperty(key) && typeof obj[key] === 'object' && obj[key] !== null) {
            var found = deepFindUrl(obj[key], depth + 1);
            if (found) return found;
        }
    }
    return '';
}


// ═══════════════════════════════════════════════════════════════════════════════
//  CONTROLLER
// ═══════════════════════════════════════════════════════════════════════════════
app.controller('PaymentLinkController', function ($scope, $http) {

    $scope.disableCopy = true;
    $scope.isSubmitting = false;
    $scope.paymentLink = '';

    // ── Validation errors from Easebuzz API ─────────────────────────────
    $scope.fieldErrors = {};
    $scope.apiErrorMessage = '';

    // ── Gateway: Easebuzz only ──────────────────────────────────────────
    $scope.paymentGateway = 'easebuzz';

    $scope.uid = GetParameterValues('uid');
    $scope.txnsuffix = GetParameterValues('suffix');

    $scope.CreatedPaymentLink = [];
    $scope.StateList = [];
    $scope.PaymentNature = [];

    // ── Easebuzz-only fields ─────────────────────────────────────────────
    $scope.ebExpiryDate = getDateString(30);
    $scope.ebMessage = 'Payment Link';


    // ═══════════════════════════════════════════════════════════════════════
    //  FIELD ERROR HELPERS
    // ═══════════════════════════════════════════════════════════════════════

    $scope.clearFieldError = function (field) {
        if ($scope.fieldErrors[field]) {
            delete $scope.fieldErrors[field];
        }
        if (Object.keys($scope.fieldErrors).length === 0) {
            $scope.apiErrorMessage = '';
        }
    };

    function clearAllErrors() {
        $scope.fieldErrors = {};
        $scope.apiErrorMessage = '';
    }

    function parseAndSetErrors(ebResp) {
        clearAllErrors();
        var errorLines = [];

        // Case 1: errors array with field + message
        if (ebResp.errors && Array.isArray(ebResp.errors) && ebResp.errors.length > 0) {
            for (var i = 0; i < ebResp.errors.length; i++) {
                var err = ebResp.errors[i];
                var formField = mapEasebuzzField(err.field || err.param || err.key || '');
                var errMsg = err.message || err.msg || err.error || 'Invalid value';
                if (formField) {
                    $scope.fieldErrors[formField] = errMsg;
                    errorLines.push(getFieldLabel(formField) + ': ' + errMsg);
                } else {
                    errorLines.push(errMsg);
                }
            }
        }
        // Case 2: error_data object { field: "message" }
        else if (ebResp.error_data && typeof ebResp.error_data === 'object') {
            var errorData = ebResp.error_data;
            for (var key in errorData) {
                if (errorData.hasOwnProperty(key)) {
                    var formField2 = mapEasebuzzField(key);
                    var errMsg2 = Array.isArray(errorData[key]) ? errorData[key].join(', ') : errorData[key];
                    $scope.fieldErrors[formField2] = errMsg2;
                    errorLines.push(getFieldLabel(formField2) + ': ' + errMsg2);
                }
            }
        }
        // Case 3: Just a message string
        else if (ebResp.message || ebResp.msg || ebResp.error) {
            var msg = ebResp.message || ebResp.msg || ebResp.error;
            errorLines.push(msg);
            var msgLower = (msg || '').toLowerCase();
            if (msgLower.indexOf('name') > -1) {
                $scope.fieldErrors['customer_name'] = msg;
            } else if (msgLower.indexOf('email') > -1) {
                $scope.fieldErrors['customer_email'] = msg;
            } else if (msgLower.indexOf('phone') > -1 || msgLower.indexOf('mobile') > -1) {
                $scope.fieldErrors['customer_mobile'] = msg;
            } else if (msgLower.indexOf('amount') > -1) {
                $scope.fieldErrors['indent_amount'] = msg;
            } else if (msgLower.indexOf('expiry') > -1 || msgLower.indexOf('date') > -1) {
                $scope.fieldErrors['ebExpiryDate'] = msg;
            }
        }

        if (errorLines.length > 0) {
            $scope.apiErrorMessage = errorLines.join('\n');
        } else {
            $scope.apiErrorMessage = 'Something went wrong. Please check your inputs and try again.';
        }

        return errorLines.join('\n') || $scope.apiErrorMessage;
    }


    // ═══════════════════════════════════════════════════════════════════════
    //  FORM INIT & RESET
    // ═══════════════════════════════════════════════════════════════════════
    function getCleanFormObj() {
        return {
            customer_name: '', customer_mobile: '', customer_email: '',
            indent_amount: '', remarks: '',
            user_id: $scope.uid, suffix: $scope.txnsuffix,
            state: '', location: '', paymentType: ''
        };
    }

    $scope.PaymentLinkObj = getCleanFormObj();

    $scope.resetForm = function () {
        $scope.PaymentLinkObj = getCleanFormObj();
        $scope.ebExpiryDate = getDateString(30);
        $scope.ebMessage = 'Payment Link';
        $scope.paymentGateway = 'easebuzz';
        $scope.disableCopy = true;
        $scope.paymentLink = '';
        $scope.isSubmitting = false;
        clearAllErrors();
        if ($scope.myForm) {
            $scope.myForm.$setPristine();
            $scope.myForm.$setUntouched();
        }
    };


    // ═══════════════════════════════════════════════════════════════════════
    //  COPY TO CLIPBOARD — modern API with fallback
    // ═══════════════════════════════════════════════════════════════════════
    $scope.copyToClipboard = function (name) {
        var textToCopy = decodeURI(name || '');
        if (!textToCopy) {
            swal({ title: 'Error', text: 'No link to copy', icon: 'warning', timer: 1500, buttons: false });
            return;
        }

        if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(textToCopy).then(function () {
                swal({ title: 'Copied!', text: 'Payment link copied to clipboard', icon: 'success', timer: 1500, buttons: false });
            }, function () {
                fallbackCopy(textToCopy);
            });
        } else {
            fallbackCopy(textToCopy);
        }
    };

    function fallbackCopy(text) {
        try {
            var copyElement = document.createElement("textarea");
            copyElement.value = text;
            copyElement.style.position = 'fixed';
            copyElement.style.left = '-9999px';
            copyElement.style.opacity = '0';
            document.body.appendChild(copyElement);
            copyElement.focus();
            copyElement.select();
            document.execCommand('copy');
            document.body.removeChild(copyElement);
            swal({ title: 'Copied!', text: 'Payment link copied to clipboard', icon: 'success', timer: 1500, buttons: false });
        } catch (e) {
            swal({ title: 'Copy Failed', text: 'Please copy manually: ' + text, icon: 'info' });
        }
    }


    // ═══════════════════════════════════════════════════════════════════════
    //  SUBMIT — Easebuzz only
    // ═══════════════════════════════════════════════════════════════════════
    $scope.submitPayment = function () {
        if ($scope.isSubmitting) return;
        clearAllErrors();
        $scope.submitEasebuzz();
    };


    // ═══════════════════════════════════════════════════════════════════════
    //  EASEBUZZ SUBMIT
    //
    //  Step 1 → AddonlinePaymentHistory → t_OnlinePaymentHistory → TXN_ID
    //  Step 2 → CreatePaymentLink API → Easebuzz link created
    //  Step 3 → UpdatePaymentLink → updates payment_link in DB
    // ═══════════════════════════════════════════════════════════════════════
    $scope.submitEasebuzz = function () {
        $scope.isSubmitting = true;
        clearAllErrors();

        var operations = [
            { type: 'sms', template: 'Default sms template' },
            { type: 'email', template: 'Default email template' },
            { type: 'whatsapp', template: 'Default whatsapp template' }
        ];

        // ════════════════════════════════════════════════════════════════
        //  STEP 1: Save to t_OnlinePaymentHistory → Get TXN_ID
        // ════════════════════════════════════════════════════════════════
        $http({
            url: '/api/WebApi/AddonlinePaymentHistory',
            method: 'POST',
            headers: { 'Content-type': 'application/json' },
            data: $scope.PaymentLinkObj
        }).then(function (response) {

            var resp = null;
            try {
                resp = typeof response.data === 'string' ? JSON.parse(response.data) : response.data;
            } catch (e) {
                $scope.isSubmitting = false;
                $scope.apiErrorMessage = 'Invalid response while generating TXN ID';
                swal({ title: 'Error', text: $scope.apiErrorMessage, icon: 'error' });
                return;
            }

            if (!resp || !resp[0]) {
                $scope.isSubmitting = false;
                $scope.apiErrorMessage = 'Failed to generate TXN ID';
                swal({ title: 'Error', text: $scope.apiErrorMessage, icon: 'error' });
                return;
            }

            // ── Extract TXN_ID ──────────────────────────────────────────
            // Works with both old SP (URL in link) and new SP (TXN_ID directly)
            var txnId = '';
            if (resp[0].TXN_ID) {
                txnId = resp[0].TXN_ID;
            } else if (resp[0].txn_id) {
                txnId = resp[0].txn_id;
            } else if (resp[0].link) {
                // Old SP returns full URL — extract last segment
                var linkParts = resp[0].link.split('/');
                txnId = linkParts[linkParts.length - 1];
            }

            // Safety: if txnId still has URL parts, extract last segment
            if (txnId && txnId.indexOf('/') > -1) {
                var parts = txnId.split('/');
                txnId = parts[parts.length - 1];
            }

            var fullName = resp[0].full_name || resp[0].Full_Name || '';

            //console.log('[PaymentLink] Step 1 — TXN_ID:', txnId, 'Full Name:', fullName);

            if (!txnId) {
                $scope.isSubmitting = false;
                $scope.apiErrorMessage = 'Failed to generate TXN ID';
                swal({ title: 'Error', text: $scope.apiErrorMessage, icon: 'error' });
                return;
            }

            // ════════════════════════════════════════════════════════════
            //  STEP 2: Call Easebuzz CreatePaymentLink
            // ════════════════════════════════════════════════════════════
            var payload = {
                token: CONFIG.EASEBUZZ_TOKEN,
                merchant_txn: txnId,
                amount: parseFloat($scope.PaymentLinkObj.indent_amount),
                name: $scope.PaymentLinkObj.customer_name,
                email: $scope.PaymentLinkObj.customer_email,
                phone: $scope.PaymentLinkObj.customer_mobile,
                message: $scope.ebMessage || 'Payment Link',
                expiry_date: $scope.ebExpiryDate || getDateString(30),
                udf1: $scope.PaymentLinkObj.paymentType || '',
                udf2: fullName,
                udf3: $scope.PaymentLinkObj.location || '',
                udf4: $scope.PaymentLinkObj.state || '',
                udf5: $scope.PaymentLinkObj.remarks || '',
                operation: operations
            };

            $http({
                url: CONFIG.EASEBUZZ_API_URL,
                method: 'POST',
                headers: { 'Content-type': 'application/json' },
                data: payload
            }).then(function (ebResponse) {

                var ebResp = ebResponse.data;

                // ══════════════════════════════════════════════════════
                //  DEBUG: Log full Easebuzz response to console
                //  Open browser DevTools → Console tab to see this
                // ══════════════════════════════════════════════════════
                //console.log('[PaymentLink] Step 2 — Full Easebuzz Response:', JSON.stringify(ebResp, null, 2));

                if (ebResp.success) {
                    clearAllErrors();

                    // ── Robust link extraction — tries every possible path ──
                    var ebLink = extractPaymentLink(ebResp);

                    //console.log('[PaymentLink] Step 2 — Extracted Link:', ebLink);

                    if (ebLink) {
                        $scope.paymentLink = ebLink;
                        $scope.disableCopy = false;

                        // ════════════════════════════════════════════════
                        //  STEP 3: Update payment_link in DB
                        // ════════════════════════════════════════════════
                        $http({
                            url: '/api/WebApi/UpdateEasebuzzLink',
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            data: { TXN_ID: txnId, payment_link: ebLink }
                        }).then(function () {
                            $scope.GetPayments();
                        }, function () {
                            $scope.GetPayments();
                        });

                        $scope.isSubmitting = false;
                        swal({
                            title: 'Success',
                            text: 'Payment link created!\n\n' + ebLink,
                            icon: 'success'
                        });
                    } else {
                        // ── Success but no link found in response ────────
                        // Refresh list — link might have been saved by the API directly
                        $scope.GetPayments();
                        $scope.isSubmitting = false;
                        console.warn('[PaymentLink] Success response but no link found. Full response:', ebResp);
                        swal({
                            title: 'Partial Success',
                            text: 'Payment link was created but could not be retrieved from the response. Please copy from the list below.',
                            icon: 'warning'
                        });
                    }
                } else {
                    // ══════════════════════════════════════════════════════
                    //  VALIDATION ERROR HANDLING
                    // ══════════════════════════════════════════════════════
                    $scope.isSubmitting = false;
                    var errMsg = parseAndSetErrors(ebResp);
                    swal({ title: 'Validation Error', text: errMsg, icon: 'error' });
                }

            }, function (ebError) {
                $scope.isSubmitting = false;
                console.error('[PaymentLink] Step 2 — HTTP Error:', ebError);

                var errMsg = 'Failed to create payment link.';
                if (ebError.data) {
                    errMsg = parseAndSetErrors(ebError.data);
                } else if (ebError.status === 0) {
                    errMsg = 'Unable to connect to payment gateway. Please check your internet connection.';
                } else if (ebError.status === 408 || ebError.status === 504) {
                    errMsg = 'Payment gateway request timed out. Please try again.';
                } else if (ebError.status >= 500) {
                    errMsg = 'Payment gateway server error. Please try again later.';
                }
                $scope.apiErrorMessage = errMsg;
                swal({ title: 'Error', text: errMsg, icon: 'error' });
            });

        }, function (error) {
            $scope.isSubmitting = false;
            console.error('[PaymentLink] Step 1 — HTTP Error:', error);
            $scope.apiErrorMessage = 'Failed to save payment record';
            swal({ title: 'Error', text: $scope.apiErrorMessage, icon: 'error' });
        });
    };


    // ═══════════════════════════════════════════════════════════════════════
    //  DATA LOADERS
    // ═══════════════════════════════════════════════════════════════════════

    $scope.paramGetPayments = { 'uid': $scope.uid, 'suffix': $scope.txnsuffix };

    $scope.GetPayments = function () {
        $http({
            url: '/api/WebApi/GetPayments',
            method: 'post',
            headers: { 'Content-type': 'application/json' },
            data: $scope.paramGetPayments
        }).then(function (response) {
            try {
                var list = typeof response.data === 'string' ? JSON.parse(response.data) : response.data;
                if (Array.isArray(list)) {
                    for (var i = 0; i < list.length; i++) {
                        var link = (list[i].link || list[i].payment_link || '').toLowerCase();
                        if (link.indexOf('easebuzz') > -1 || link.indexOf('easy_collect') > -1 || link.indexOf('easycollect') > -1) {
                            list[i].gateway = 'Easebuzz';
                        } else if (link) {
                            list[i].gateway = 'PayU';
                        } else {
                            list[i].gateway = '-';
                        }
                    }
                    $scope.CreatedPaymentLink = list;
                } else {
                    $scope.CreatedPaymentLink = [];
                }
            } catch (e) {
                $scope.CreatedPaymentLink = [];
            }
        }, function () {
            $scope.CreatedPaymentLink = [];
        });
    };

    $scope.GetStateList = function () {
        $http({
            url: '/api/WebApi/GetStateList',
            method: 'post',
            headers: { 'Content-type': 'application/json' }
        }).then(function (response) {
            try {
                $scope.StateList = typeof response.data === 'string' ? JSON.parse(response.data) : response.data;
            } catch (e) {
                $scope.StateList = [];
            }
        }, function () {
            $scope.StateList = [];
        });
    };

    $scope.GetPaymentNature = function () {
        $http({
            url: '/api/WebApi/GetPaymentNature',
            method: 'post',
            headers: { 'Content-type': 'application/json' }
        }).then(function (response) {
            try {
                $scope.PaymentNature = typeof response.data === 'string' ? JSON.parse(response.data) : response.data;
            } catch (e) {
                $scope.PaymentNature = [];
            }
        }, function () {
            $scope.PaymentNature = [];
        });
    };

    // ── Init ──────────────────────────────────────────────────────────────
    $scope.GetPayments();
    $scope.GetStateList();
    $scope.GetPaymentNature();
});