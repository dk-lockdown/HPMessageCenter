//NOTE: 所有控件input[type=text]关闭自动完成autocomplete="off",否则KO无法正常获取值
/*
    KO view manager
*/
(function () {
    function ViewManager() {
        this._view = {};
    }

    ViewManager.prototype = {
        view: function (viewName, view) {
            if (view) {
                if (!this._view[viewName]) {
                    this._view[viewName] = view;
                } else {
                    //if (console) {
                    //    console.warn(viewName + " is defined");
                    //}
                }
                return this;
            } else {
                return this._view[viewName];
            }
        },
        New: function (viewName) {
            var view = this.view(viewName);
            if (view) {
                return new view();
            } else {
                throw "not found view : " + viewName;
            }
        },
        instance: function (selector, instance) {
            var $ele = $(selector);
            if (instance) {
                $ele.data("view", instance);
                return this;
            } else {
                return $ele.data("view");
            }
        }
    };
    if (!window.$vm) {
        window.$vm = new ViewManager();
    } else {
        throw "$vm is defined";
    }

})();


/*
    # ko extender 
    # trim
*/
ko.extenders.trim = function (target, ops) {
    if (ops) {
        var value = target();
        target($.trim(value));
    }
    return target;
};

/*
    # ko bind handler.
    # if value ==1 or value == true then output '是' else '否'
*/
var yesnoHandler = function (element, valueAccessor, allBindings, viewModel, bindingContext) {
    var value = ko.unwrap(valueAccessor());
    var result;
    if (/^true$/i.test(value) || value == 1) {
        result = '是';
    }
    else {
        result = '否';
    }
    $(element).text(result);
}

ko.bindingHandlers.my97date = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.unwrap(valueAccessor());
        var el = $(element);
        el.val(value);
        el.on("blur", {
            valueAccessor: valueAccessor, allBindings: allBindings, viewModel: viewModel
        }, function (event) {
            var self = $(this);
            valueAccessor()(self.val());
        });

    }, update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.unwrap(valueAccessor());
        var el = $(element);
        el.val(value);
    }
};

/*
    # ko bind handler.
    # if value ==1 then output '可用' else '不可用'
*/
var commonStatusHandler = function (element, valueAccessor, allBindings, viewModel, bindingContext) {
    var value = ko.unwrap(valueAccessor());
    var result;
    if (value == 1) {
        result = '可用';
    } else {
        result = '不可用';
    }
    $(element).text(result);
};
ko.bindingHandlers.yesNo = { init: yesnoHandler, update: yesnoHandler };
ko.bindingHandlers.commonStatus = { init: commonStatusHandler, update: commonStatusHandler };
ko.bindingHandlers.sortBy = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        if (viewModel instanceof GridView) {
            var field = valueAccessor();
            var ele = $(element);
            ele.addClass("sorting");
            ele.click(function () {
                if (field != viewModel.sortField()) {
                    if (viewModel._preSortEle) {
                        //clear sorting style
                        $(viewModel._preSortEle).attr("class", "sorting");
                        viewModel.sortField("");
                        viewModel.sortType("");
                    }
                    viewModel._preSortEle = this;
                }
                var self = $(this);
                var type = self.hasClass("sorting_asc") ? "desc" : "asc";
                self.attr("class","sorting_" + type);
                viewModel.sortField(field);
                viewModel.sortType(type);
            });
        }
    }
};
ko.bindingHandlers.enterkey = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        $(element).keypress(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode);
            if (keyCode === 13) {
                callback.call(viewModel);
                return false;
            }
            return true;
        });
    }
};

/*
    # 事件机制使用的Jquery的事件,在使用的时候尽量避免自己定义的事件名称和原始的事件名称一样,如:click等
*/
function BaseView() {
    this.ele = document.body;
    this.$validations = [];
    this.$validCount = 0;
    /*
        0:defined
        1:ready
    */
    this.$validattionStatus = ko.observable(0);

    this.enums = {
        CommonStatus: [{
            text: "不可用",
            value: 0
        }, {
            text: "可用",
            value: 1
        }],
        YesNo: [{
            text: "否",
            value: false
        }, {
            text: "是",
            value: true
        }]
    };
}
BaseView.prototype = {
    $valid: function () {
        var vstatus = this.$validattionStatus();
        if (vstatus != 1) {
            this.$validattionStatus(1);
        }
        return this.$validCount>=0;
    },
    $clearValidation: function () {
        this.$validCount = 0;
        this.$validattionStatus(0);
        return this;
    },
    /*
        NOTE: please try to avoid use the same to original event name , such as click/reload/ok/cancel etc. 
    */
    on: function (name, fn) {
        $(this).on(name, fn);
        return this;
    },
    off: function (name, handler) {
        if (handler) {
            $(this).off(name, handler);
        } else {
            $(this).unbind();
        }
        return this;
    },
    trigger: function (name) {
        var args = [];
        for (var i = 1; i < arguments.length; i++) {
            args.push(arguments[i]);
        }
        $(this).trigger(name, args);
        return this;
    },
    mapping: function (jsonData, deep) {
        if (deep) {
            ko.mapping.fromJS(BaseView.deepMapping(jsonData), {}, this);
        }
        else {
            ko.mapping.fromJS(jsonData, {}, this);
        }
        return this;
    },
    setEle: function (selector) {
        this.ele = $(selector)[0];
        return this;
    },
    rebinding: function (data, deep) {
        if (data instanceof Array) {
            throw "data can't be array";
        }
        this.unbinding();
        this.mapping(data, deep);
        this.binding();
        return this;
    },
    binding: function (selector) {
        if (selector) {
            this.setEle(selector);
        }
        this.trigger("prebinding", this);
        ko.applyBindings(this, this.ele);
        this.trigger('after_binding', this);
        return this;
    },
    unbinding: function () {
        if (this.ele) {
            ko.cleanNode(this.ele);
        }
        return this;
    }

};
BaseView.deepMapping = function (jsonData) {
    function hasArr(obj) {
        for (var key in obj) {
            if (obj.hasOwnProperty(key)) {
                if (obj[key] instanceof Array) {
                    return true;
                }
            }
        }
        return false;
    }
    function mapping(obj) {
        if (obj instanceof Array) {
            for (var i = 0; i < obj.length; i++) {
                if (hasArr(obj[i])) {
                    obj[i] = mapping(obj[i]);
                }
                else {
                    obj[i] = ko.mapping.fromJS(obj[i]);
                }
            }
            return ko.observableArray(obj);
        }
        else if (typeof obj == "object") {
            return ko.mapping.fromJS(obj);
        }
        return ko.observable(obj);
    }

    return mapping(jsonData);
};
BaseView.applyData = function (view, data) {
    function set(view, data) {
        for (var key in data) {
            if (view[key] && typeof view[key] === "function") {
                if (data[key] instanceof Array) {
                    view[key](BaseView.deepMapping(data[key])());
                } else if (data[key] instanceof Object) {
                    view[key](BaseView.deepMapping(data[key]));
                }
                else {
                    view[key](data[key]);
                }
            }

        }
    }
    set(view, data);
}
BaseView.getData = function (selector) {
    return ko.dataFor($(selector).get(0));
};

function GridView() {
    BaseView.call(this);
    this.filter = ko.mapping.fromJS({
        PageIndex: 0,
        PageSize: 10
    });
    this.sortField = ko.observable("");
    this.sortType = ko.observable("");
    this.sortType.subscribe(function (newVal) {
        this.filter.PageIndex(0);
        this.trigger("page_change", this.filter.PageIndex());
    },this);
    this.sortText = ko.pureComputed(function() {
        return this.sortField() + " " + this.sortType();
    }, this);
    this.list = ko.observableArray();
    this.total = ko.observable(0);
    this.totalPage = ko.computed(function () {
        return Math.ceil(this.total() / this.filter.PageSize());
    }, this);
    this._oldPageSize = 10;
    this.filter.PageSize.subscribe(function (oldVal) {
        this._oldPageSize = oldVal;
    }, this, "beforeChange");
    this.filter.PageSize.subscribe(function (newValue) {
        if (newValue != this._oldPageSize) {
            this.filter.PageIndex(0);
            this.trigger("page_change", 0);
        }
    }, this);

    this.pageItem = ko.pureComputed(function () {
        
        var count = 5;
        var result = [];
        var pageIndex = this.filter.PageIndex();
        var totalPage = this.totalPage();
        var start = pageIndex - count;
        if (start < 0 ) {
            start = 0;
        }
        var end = start + count;
        if (end >= totalPage) {
            end = totalPage-1;
        }
        for (; start <= end; start++) {
            result.push(start+1);
        }
        return result;
    }, this);

}

GridView.prototype = new BaseView();
GridView.prototype.next = function () {
    var index = this.filter.PageIndex();
    if (index === (this.totalPage() - 1)) return;
    this.filter.PageIndex(++index);
    this.trigger("page_change", index);
};
GridView.prototype.prev = function () {
    var index = this.filter.PageIndex();
    if (index === 0) return;
    this.filter.PageIndex(--index);
    this.trigger("page_change", index);
};
GridView.prototype.goto = function (pageIndex) {
    var index = this.filter.PageIndex();
    if (index === pageIndex) return;
    this.filter.PageIndex(pageIndex);
    this.trigger("page_change", pageIndex);
};
GridView.prototype.resetFilter = function () {
    this.total(0);
    this.filter.PageIndex(0);
    this.sortField("");
    this.sortType("");
    this.trigger("reset_filter", this.filter);
};


function BaseFormView() {
    BaseView.call(this);
    this.$canEdit = ko.observable(false);
    this.$oldData = null;
}
BaseFormView.prototype = new BaseView();
BaseFormView.prototype.fnEdit = function ($data) {
    this.$canEdit(true);
    this.$oldData = ko.mapping.toJS($data);
    this.trigger("edit", this);
};
BaseFormView.prototype.fnSave = function ($data) {
    //this.$canEdit(false);
    this.trigger("save", $data);
};
BaseFormView.prototype.fnCancel = function () {
    this.$canEdit(false);
    this.mapping(this.$oldData);
    this.$oldData = null;
    this.trigger("cancel", this);
};


Date.prototype.format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, 
        "d+": this.getDate(), 
        "h+": this.getHours(), 
        "m+": this.getMinutes(), 
        "s+": this.getSeconds(), 
        "q+": Math.floor((this.getMonth() + 3) / 3), 
        "S": this.getMilliseconds() 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
};


//ko 验证
//ko.observable.fn.removePattern = function () {
//    $.each(this._subscribed, function(index, item) {
//        item.dispose();
//    });
//    delete this.hasError;
//    delete this.errorMessage;
//};
/*
    # ko extender form validation
    example:{
        rules:[{
            rule:/^.+$/,
            message:"this field is required"
        }],
        owner:this
    }
*/
//ko.extenders.pattern = function (target, ops) {

//    var owner = ops.owner;
//    if (!owner) {
//        throw "owner is required when set ko extender pattern";
//    }

//    target.hasError = ko.observable(false);
//    target.errorMessage = ko.observable("");
//    target._subscribed = [];
//    //0:defined,1:ready
//    target._status = ko.observable(0);
//    owner.$validattionStatus.subscribe(function (newVal) {
//        if (newVal === 1) {
//            function validate(newValue) {
//                var i = 0,
//                    success,
//                    len = ops.rules.length;
//                for (; i < len; i++) {
//                    if (ops.rules[i].rule instanceof RegExp) {
//                        success = ops.rules[i].rule.test(newValue != null ? newValue.toString() : "");
//                    }
//                    if (ops.rules[i].rule instanceof Function) {
//                        success = ops.rules[i].rule( newValue);
//                    }
//                    if (!success) {
//                        this.hasError(true);
//                        this.errorMessage(ops.rules[i].message);
//                        return;
//                    }
//                }
//                this.hasError(false);
//                this.errorMessage("");
//            }

//            if (this.hasError) {
//                this._subscribed.push(this.hasError.subscribe(function(newVal) {
//                    if (newVal) {
//                        owner.$validCount--;
//                    } else {
//                        owner.$validCount++;
//                    }
//                }, this));
//                this._subscribed.push(this.subscribe(validate.bind(this)));
//                validate.bind(this)(this());
//            }


//        } else {
//            $.each(this._subscribed, function (index, item) {
//                item.dispose();
//            });
//            if (this.hasError) {
//                this.hasError(false);
//                this.errorMessage("");
//            }
//            this._subscribed = [];

//        }
//    }, target);

//    owner.$validations.push(target);

//    return target;
//};