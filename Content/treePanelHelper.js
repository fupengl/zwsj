var getSelectedNode = function (treepanel) {
    if (hasSelectedNode(treepanel)) {
        return treepanel.getSelectionModel().getSelection()[0];
    }
    return null;
};

var getSelectedNodeId = function (treepanel) {
    var selectedNode = getSelectedNode(treepanel);
    if (selectedNode == null) {
        return -1;
    }
    var nodeId = selectedNode.get('id');
    if (nodeId == undefined) {
        return -1;
    }
    return nodeId;
    //return selectedNode.getId();
    //    if (hasSelectedNode(treepanel)) {
    //        return treepanel.getSelectionModel().getSelection()[0].getId();
    //    }
    //    return -1;
};

var getSelectedNodeTreePath = function (treepanel) {
    var selectedNode = getSelectedNode(treepanel);
    if (selectedNode == null) {
        return "";
    }

    var treePath = selectedNode.get('treePath');
    if (treePath == undefined) {
        return "";
    }
    return treePath;
};

var hasSelectedNode = function (treepanel) {
    return treepanel.getSelectionModel().getSelection().length > 0;
};

var beforeTreeAddCheck = function (treepanel) {
    if (!hasSelectedNode(treepanel)) {
        Ext.Msg.alert('提示', '请先选择节点.');
        return false;
    }

    return true;
};

var isSystemRecordNode = function (node) {
    var recordType = node.data['recordType'];
    if (recordType == undefined) {
        return false;
    }
    return recordType == "System";
};

var beforeTreeEditCheck = function (treepanel) {
    if (!hasSelectedNode(treepanel)) {
        Ext.Msg.alert('提示', '请先选择节点.');
        return false;
    }
    var selectedNode = getSelectedNode(treepanel);

    if (isSystemRecordNode(selectedNode)) {
        Ext.Msg.alert('提示', '系统数据不允许修改.');
        return false;
    }

    return true;
};

//var beforeTreeDeleteCheck = function(treepanel) {
//    if (!hasSelectedNode(treepanel)) {
//        Ext.Msg.alert('提示', '请先选择要删除的节点.');
//        return false;
//    }

//    var selectedNode = getSelectedNode(treepanel);
//    if (selectedNode.hasChildNodes()) {
//        Ext.Msg.alert('提示', '请先删除子节点.');
//        return false;
//    }

//    Ext.MessageBox.confirm("提示", "是否删除选中节点?", function(button, text) {
//        if (button == 'yes') {
//            return true;
//        } else {
//            return false;
//        }
//    });
//};

var deleteNode = function (treepanel, controller, action) {
    if (!hasSelectedNode(treepanel)) {
        Ext.Msg.alert('提示', '请先选择要删除的节点.');
        return;
    }

    var selectedNode = getSelectedNode(treepanel);

    if (isSystemRecordNode(selectedNode)) {
        Ext.Msg.alert('提示', '系统数据不允许删除.');
        return;
    }
    if (selectedNode.hasChildNodes()) {
        Ext.Msg.alert('提示', '请先删除子节点.');
        return;
    }

    Ext.MessageBox.confirm("提示", "是否删除选中节点?", function (button, text) {
        if (button == 'yes') {
            Ext.net.DirectMethod.request(
                        {
                            url: getUrl(controller, action),
                            params: {
                                id: getSelectedNodeId(treepanel)
                            },
                            success: function (result) {
                                var parentNode = selectedNode.parentNode;
                                selectedNode.remove();
                                
                                if (parentNode.childNodes.length == 0) {
                                    parentNode.leaf = true;
                                }
                            },
                            failure: function (errorMessage) {
                                Ext.Msg.alert(errorMessage);
                            }
                        }
                    );
        }
    });
};
var deleteNodeWithouCheckChild = function(treepanel, controller, action) {
    if (!hasSelectedNode(treepanel)) {
        Ext.Msg.alert('提示', '请先选择要删除的节点.');
        return;
    }

    var selectedNode = getSelectedNode(treepanel);

    if (isSystemRecordNode(selectedNode)) {
        Ext.Msg.alert('提示', '系统数据不允许删除.');
        return;
    }

    Ext.MessageBox.confirm("提示", "是否删除选中节点?", function(button, text) {
        if (button == 'yes') {
            Ext.net.DirectMethod.request(
                {
                    url: getUrl(controller, action),
                    params: {
                        id: getSelectedNodeId(treepanel)
                    },
                    success: function(result) {
                        var parentNode = selectedNode.parentNode;
                        selectedNode.remove();

                        if (parentNode.childNodes.length == 0) {
                            parentNode.leaf = true;
                        }
                    },
                    failure: function(errorMessage) {
                        Ext.Msg.alert(errorMessage);
                    }
                }
            );
        }
    });
};
var getNodeData = function(node) {
    return node.data;
};