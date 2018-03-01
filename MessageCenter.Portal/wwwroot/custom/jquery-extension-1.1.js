(function ($) {
    $.initPlaceholder = function () {
        $("input,textarea,select[multiple]").each(function () {
            var label = $(this).parent().prev(":first");
            if (label.is("label")) {
                if ($(this).is("select")) {
                    $(this).attr("title", label.text());
                } else {
                    $(this).attr("placeholder", label.text());
                }
            }
        });
    };

    //form : Jquery Object
    //data : JSON
    $.bindEntity = function (form, data) {
        if (!data) return;
        function _getValue(dataModel, data2) {
            var name = dataModel.shift();
            if (data2[name] && (typeof (data2[name]) == "object")) {
                return _getValue(dataModel, data2[name]);
            } else {
                return data2[name];
            }
        }

        form.find("[data-model]").each(function () {
            var model = $(this).attr("data-model").split(".");
            var val = _getValue(model, data);
            if ($(this).is("select") && $(this).prop("multiple")) {
                if (val) {
                    $(this).val(val.split(","));
                }
            } else {
                $(this).val(val);
            }
        });
        var bsSelect = $('.bs-select');
        if (bsSelect.length > 0 && bsSelect.selectpicker) {
            bsSelect.selectpicker('refresh');
        }

    };

    //form : Jquery Object
    function buildEntity(form) {
        var obj = {};
        form.find("[data-model]").each(function () {
            var name = $(this).attr("data-model");
            var names = name.split(".");
            var temp;
            var val;
            if ($(this).attr("type") == "checkbox"
                || $(this).attr("type") == "radio") {
                if ($(this).prop("checked")) {
                    val = $(this).val();
                }
            }
            else {
                val = $.trim($(this).val());
            }
            if (names.length > 1) {
                for (var i = 0; i < names.length; i++) {
                    if (temp) {
                        if (!temp[names[i]]) {
                            if (i == (names.length - 1)) temp[names[i]] = val;
                            else temp[names[i]] = {};
                        }
                        temp = temp[names[i]];
                    } else {
                        if (!obj[names[i]]) {
                            if (i == (names.length - 1)) obj[names[i]] = val;
                            else obj[names[i]] = {};
                        }
                        temp = obj[names[i]];
                    }
                }
            } else {
                obj[name] = val;
            }

        });
        return obj;
    }

    $.buildEntity = buildEntity;

    //页面导出Excel:
    $.exportExcel = function (action, params) {
        var form = $("<form>");//定义一个form表单
        form.attr("style", "display:none");
        form.attr("target", "");
        form.attr("method", "post");
        form.attr("action", action);
        $("body").append(form);//将表单放置在web中
        if (params && params.length > 0) {

            for (var i = 0 ; i < params.length; i++) {
                var input1 = $("<input>");
                input1.attr("type", "hidden");
                input1.attr("name", params[i].name);
                input1.attr("value", params[i].value);
                form.append(input1);
            }
        }
        form.submit();//表单提交 
    };

    //不带分页的普通的Grid
    $.grid = function (src, options) {
        options = $.extend(true, {
            autoWidth: false,
            paging: false,
            serverSide: false,
            searching: false,
            info: false,
            ordering: false,
            dom: '<"top">rt<"bottom"ilp><"clear">',
            language: { // language settings
                "infoEmpty": "暂无相关记录",
                "emptyTable": "暂无相关记录",
                "zeroRecords": "暂无相关记录"
            }
        }, options);

        var table = src.DataTable(options);
        $("#" + src.attr("id") + " .group-checkable").change(function () {
            var set = $("#" + src.attr("id") + ' tbody > tr > td input[type="checkbox"]');
            var checked = $(this).is(":checked");
            $(set).each(function () {
                $(this).attr("checked", checked);
            });
            //$('input[type="checkbox"].minimal, input[type="radio"].minimal').iCheck({
            //    checkboxClass: 'icheckbox_minimal-blue',
            //    radioClass: 'iradio_minimal-blue'
            //})
        })
        return {
            getTable: function () {
                return table;
            },
            addRow: function (rowData) {
                table.row.add(rowData).draw();

                //$('input[type="checkbox"].minimal, input[type="radio"].minimal').iCheck({
                //    checkboxClass: 'icheckbox_minimal-blue',
                //    radioClass: 'iradio_minimal-blue'
                //})
            },
            clear: function () {
                return table.clear().draw();
            },
            getAllDatas: function () { return table.data(); },

            getSelectedRowsCount: function () {
                return $("#" + src.attr("id") + ' tbody > tr > td input[type="checkbox"]:checked').size();
            },

            getSelectedRowsValue: function () {
                var rows = [];
                $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).each(function () {
                    rows.push($(this).val());
                });

                return rows;
            },
            getSelectedRowsData: function () {
                var rows = [];
                $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).each(function () {
                    rows.push(table.row($(this).parents('tr')).data());
                });
                return rows;
            },
            deleteSelectedRows: function () {
                var rows = [];
                $("#" + src.attr("id") + ' tbody > tr > td:nth-child(1) input[type="checkbox"]:checked').each(function () {
                    rows.push($(this));
                });
                if (rows && rows.length > 0) {
                    for (var i = 0; i < rows.length; i++) {
                        table.row(rows[i].parents('tr')).remove().draw();
                    }
                }
                else {
                    swal('请先选中要删除的行!');
                }
            }
        };
    };

    //分页AjaxGrid
    $.ajaxGrid = function (options) {
        var table;
        var tableOptions = options;
        return {
            loadData: function (queryEntity) {
                if (table) {
                    table.clearAjaxParams();
                    table.addAjaxParam("queryfilter", queryEntity);
                    table.getDataTable().ajax.reload();
                }
                else {
                    table = new Datatable();
                    table.clearAjaxParams();
                    table.addAjaxParam("queryfilter", queryEntity);
                    table.init(tableOptions);
                }
            },
            getSelectedRows: function () {
                return table.getSelectedRows();
            },
            getSelectedRowsCount: function () {
                return table.getSelectedRowsCount();
            },
            getTable: function () {
                return table.getTable();
            }
        };
    };

    $.daterangePicker = function (id) {
        var date_range_picker_option = {
            "showDropdowns": true,
            "timePicker": false,
            "timePickerIncrement": 30,
            "ranges": {
                "最近 7 天": [
                    moment().subtract(6, 'days'),
                    moment()
                ],
                "最近 30 天": [
                    moment().subtract(29, 'days'),
                    moment()
                ],
                "本月": [
                    moment().startOf('month'),
                    moment().endOf('month')
                ],
                "上月": [
                    moment().subtract(1, 'month').startOf('month'),
                    moment().subtract(1, 'month').endOf('month')
                ]
            },
            "locale": {
                "format": "MM/DD/YYYY",
                "separator": " - ",
                "applyLabel": "确定",
                "cancelLabel": "取消",
                "fromLabel": "从",
                "toLabel": "到",
                "customRangeLabel": "自定义",
                "daysOfWeek": [
                    "日",
                    "一",
                    "二",
                    "三",
                    "四",
                    "五",
                    "六"
                ],
                "monthNames": [
                    "一月",
                    "二月",
                    "三月",
                    "四月",
                    "五月",
                    "六月",
                    "七月",
                    "八月",
                    "九月",
                    "十月",
                    "十一月",
                    "十二月"
                ],
                "firstDay": 1
            },
            "startDate": moment().subtract(29, 'days'),
            "endDate": moment()
        };
        var cb = function (start, end, label) {
            console.log("Created date range selected: ' + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD') + ' (predefined range: ' + label + ')");
        };

        var el = $(id).daterangepicker(date_range_picker_option, cb);

        var daterangepicker = el.data('daterangepicker');

        return {

            getStartDate: function () {
                return daterangepicker.startDate.format("YYYY/MM/DD");
            },
            getEndDate: function () {
                return daterangepicker.endDate.add(1,'days').format("YYYY/MM/DD");
            },
            reset: function ()
            {
                daterangepicker.startDate = moment().subtract(29, 'days');
                daterangepicker.endDate = moment();
            }
        };
    };

    //绑定下拉选择框数据//parm = { id: "", showAll: true, data: [], value: "" }
    $.bindSelecter = function (param) {
        var $select = $("#" + param.id);
        $select.empty();
        if (param.showAll == undefined || param.showAll) {
            $select.append("<option value=''>" + "-所有-" + "</option>");
        }
        if (param.data) {
            $.each(param.data, function (i, item) {
                $select.append("<option value='" + item.Code + "'" + (param.value == item.Code ? " selected='selected'" : "") + ">" + item.Name + "</option>");
            });
        }
        if (typeof (param.callback) == "function") {
            param.callback();
        }
    };
    //Ajax获取下拉选择框数据并绑定到指定select标签上面//parm = { id: "", showAll: true, value: "", url: "" }
    $.selecter = function (param) {
        if (param.id == undefined || param.id.length <= 0) {
            return;
        }
        if (param.url == undefined || param.url.length <= 0) {
            return;
        }
        $.ajax({
            url: param.url,
            type: "POST",
            dataType: "json",
            data: {},
            success: function (data) {
                if (data) {
                    param.data = data;
                    $.bindSelecter(param);
                } else {
                    if (typeof (param.callback) == "function") {
                        param.callback();
                    }
                }
            }
        });
    };
})(jQuery);

//ajax post 
$(document).ajaxSend(onAjaxSend)
.ajaxComplete(onComplete)
.ajaxSuccess(onSuccess)
.ajaxError(onError);

function onAjaxSend(event, xhr, settings) {
    if (settings.type.toUpperCase() == "GET") {
        if (settings.url.indexOf("?") > 0) {
            settings.url += "&t=" + new Date().getTime();
        }
        else {
            settings.url += "?t=" + new Date().getTime();
        }
    }
}
function onComplete(event, xhr, settings) {
}
function onSuccess(event, xhr, settings) {
    if (xhr.responseText != "") {
        if (settings.dataType == 'json') {
            var jsonValue = jQuery.parseJSON(xhr.responseText);
            if (!jsonValue.Success) {
                swal({ text: jsonValue.Message });
            }
        }
        else if (settings.dataType == 'html') {
            var jqError = $(xhr.responseText).filter("#MessageCenter_Exception");
            if (jqError.length > 0) {
                swal({ text: jqError.find(" #errorMessage").val() });
            }
        }
        else if (settings.dataType == 'xml') {
            var error = $(xhr.responseText).find('message');
            if (error && error.length > 0) {
                swal({ text: $(error).text() });
            }
        }
    }
}
function onError(event, xhr, settings) {
    if (xhr.status != 0) {
        var error = $(xhr.responseText).find('message');
        if (error && error.length > 0) {
            swal({ text: "请求发生错误 :" + $(error).text() });
        }
        else {
            swal({ text: "请求发生错误 :" + xhr.responseText });
        }
    }
}

Number.prototype.toFixed = function (d) {
    var s = this + "";
    if (!d) d = 0;
    if (s.indexOf(".") == -1) s += ".";
    s += new Array(d + 1).join("0");
    if (new RegExp("^(-|\\+)?(\\d+(\\.\\d{0," + (d + 1) + "})?)\\d*$").test(s)) {
        var s = "0" + RegExp.$2, pm = RegExp.$1, a = RegExp.$3.length, b = true;
        if (a == d + 2) {
            a = s.match(/\d/g);
            if (parseInt(a[a.length - 1]) > 4) {
                for (var i = a.length - 2; i >= 0; i--) {
                    a[i] = parseInt(a[i]) + 1;
                    if (a[i] == 10) {
                        a[i] = 0;
                        b = i != 1;
                    } else break;
                }
            }
            s = a.join("").replace(new RegExp("(\\d+)(\\d{" + d + "})\\d$"), "$1.$2");

        } if (b) s = s.substr(1);
        return (pm + s).replace(/\.$/, "");
    } return this + "";

};

if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (elem, startFrom) {
        var startFrom = startFrom || 0;
        if (startFrom > this.length) return -1;

        for (var i = 0; i < this.length; i++) {
            if (this[i] == elem && startFrom <= i) {
                return i;
            } else if (this[i] == elem && startFrom > i) {
                return -1;
            }
        }
        return -1;
    };
}

$.formatMoney = function (amount) {
    try {
        return Number(amount).toFixed(2);
    } catch (e) {
        return amount;
    }
};
