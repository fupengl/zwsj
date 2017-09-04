var hasSelectedRecord = function (grid) {
    return grid.getRowsValues({ selectedOnly: true }).length > 0;
};

var hasSelectedMultiRecord = function (grid) {
    return grid.getRowsValues({ selectedOnly: true }).length == 1;
};


var getSelectedRecord = function (grid) {
    return grid.getRowsValues({ selectedOnly: true })[0];
};

var getSelectedRecords = function (grid) {
    return grid.getRowsValues({ selectedOnly: true });
};

var getSelectedRowId = function (grid) {
    if (grid == undefined) {
        return -1;
    }
    if (!hasSelectedRecord(grid)) {
        return -1;
    }
    var id = getSelectedRecord(grid)['Id'];
    if (id == undefined) {
        return -1;
    }
    return id;
};

var canApprove = function(grid) {
    if (!hasSelectedRecord(grid)) {
        Ext.Msg.alert('提示', '请先选择要审核的记录.');
        return false;
    }
    var status = getFieldValue(grid, "Status");
    if (status == "待审核" || status == "退回" || status == "") {
        return true;
    } else {
        Ext.Msg.alert('提示', status+"状态不允许审核.");
        return false;
    }
};

var getWuYeBianHao = function(grid) {
    return getFieldValue(grid, 'WuYeBianHao');
};

var getFieldValue = function(grid, field) {
    if (grid == undefined) {
        return "";
    }
    if (!hasSelectedRecord(grid)) {
        return "";
    }
    var val = getSelectedRecord(grid)[field];
    if (val == undefined) {
        return "";
    }
    return val;
};

var isSystemRecord = function (selectedRecord) {
    var recordType = selectedRecord['RecordType'];
    if (recordType == undefined) {
        return false;
    }

    return recordType == "System";
//    if (selectedRecord.RecordType == undefined) {
//        return false;
//    }
//    return selectedRecord.RecordType == "System";
};

var beforeEditMultiCheck = function (grid) {
    if (!hasSelectedMultiRecord(grid)) {
        Ext.Msg.alert('提示', '请先选择一条要编辑的记录.');
        return false;
    }
    return true;
};

var beforeEditCheck = function(grid) {
    if (!hasSelectedRecord(grid)) {
        Ext.Msg.alert('提示', '请先选择要编辑的记录.');
        return false;
    }
    
    var selectedRecord = getSelectedRecord(grid);
    if (isSystemRecord(selectedRecord)) {
        Ext.Msg.alert('提示', '系统数据不允许编辑.');
        return false;
    }
    
    return true;
};

var deleteRows = function (grid, controller, action) {

    if (!hasSelectedRecord(grid)) {
        Ext.Msg.alert('提示', '请先选择要删除的记录.');
        return;
    }

    var selectedRecord = getSelectedRecords(grid);
    if (isSystemRecord(selectedRecord)) {
        Ext.Msg.alert('提示', '系统数据不允许删除.');
        return;
    }

    Ext.MessageBox.confirm("提示", "是否删除选中行?", function (button, text) {
        if (button == 'yes') {
            for (var i = 0; i < selectedRecord.length; i++) {
                Ext.net.DirectMethod.request(
                    {
                        url: getUrl(controller, action),
                        params: {
                            id: selectedRecord[i].Id
                        },
                        success: function (result) {


                        },
                        failure: function (errorMessage) {
                            Ext.Msg.alert(errorMessage);
                        }
                    }
                );

            }
            grid.getStore().reload();
        }
    }
    );
};

var deleteRow = function (grid, controller, action) {
    if (!hasSelectedRecord(grid)) {
        Ext.Msg.alert('提示', '请先选择要删除的记录.');
        return;
    }

    var selectedRecord = getSelectedRecord(grid);
    if (isSystemRecord(selectedRecord)) {
        Ext.Msg.alert('提示', '系统数据不允许删除.');
        return;
    }
    
    Ext.MessageBox.confirm("提示", "是否删除选中行?", function (button, text) {
        if (button == 'yes') {
            Ext.net.DirectMethod.request(
                        {
                            url: getUrl(controller, action),
                            params: {
                                id: getSelectedRowId(grid)
                            },
                            success: function (result) {
                                grid.getStore().reload();
                            },
                            failure: function (errorMessage) {
                                Ext.Msg.alert(errorMessage);
                            }
                        }
                    );
        }
    });
};

var deleteAllRows = function (grid, controller, action) {


    Ext.MessageBox.confirm("提示", "是否删除全部数据?", function (button, text) {
        if (button == 'yes') {
            Ext.net.DirectMethod.request(
        {
            url: getUrl(controller, action),

            params: {

            },
            success: function (result) {
                grid.getStore().reload();
            },
            failure: function (errorMessage) {
                Ext.Msg.alert(errorMessage);
            }
        }
    );
                }
            });
};

function getUrl(controller, action) {
    return "/" + controller + "/" + action;
}

;

//var beforeDeleteCheck = function (grid) {
//    if (getSelectedRowId(grid) == -1) {
//        Ext.Msg.alert('提示', '请先选择要删除的记录.');
//        return false;
//    }
//    Ext.MessageBox.confirm("提示", "是否删除选中行?", function (button, text) {
//        if (button == 'yes') {
//            return true;
//        } else {
//            return false;
//        }
//    });
//};

var formValidation = function(formPanel) {
    if (formPanel.getForm().isValid()) {
        return true;
    }
    Ext.Msg.alert('提示', '数据验证失败，请检查');
    return false;
};
var clearFilters = function(grid) {
    if (grid == undefined) return;
    grid.filters.clearFilters();
};

var getFilters = function (grid) {
    //return grid.getFilterPlugin().buildQuery(grid.getFilterPlugin().getFilterData());
    return grid.getFilterPlugin().getFilterData();
};

var getSorters = function (grid) {
    return grid.getStore().getSorters();
};

var getVisibleColumns = function (grid) {
    var visibleColumns = [];

    Ext.each(grid.columns, function (c) {
        if (!c.hidden && c.dataIndex.length > 0) {
            var obj = { dataIndex: c.dataIndex, text: c.text };
            visibleColumns.push(obj);
        }
    });

    return visibleColumns;
};