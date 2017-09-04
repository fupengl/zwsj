var zindex = 10000;
function ComplexCustomOverlay(point, text, mouseoverText, bianHao) {
    this._point = point;
    this._text = text;
    this._overText = mouseoverText;
    this._bianhao = bianHao;
}
ComplexCustomOverlay.prototype = new BMap.Overlay();
ComplexCustomOverlay.prototype.initialize = function(map) {
    this._map = map;
    var div = this._div = document.createElement("div");
    div.style.position = "absolute";
    div.style.zIndex = BMap.Overlay.getZIndex(this._point.lat);
    div.style.backgroundColor = "#EE5D5B";
    div.style.border = "1px solid #BC3B3A";
    div.style.color = "white";
    div.style.height = "18px";
    //fix Bootstrap offset 18px+6px
    //div.style.height = "24px";
    div.style.padding = "2px";
    div.style.lineHeight = "18px";
    div.style.whiteSpace = "nowrap";
    div.style.MozUserSelect = "none";
    div.style.fontSize = "12px"
    var span = this._span = document.createElement("span");
    div.appendChild(span);
    span.appendChild(document.createTextNode(this._text));
    var that = this;

    var arrow = this._arrow = document.createElement("div");
    arrow.style.background = "url(../../Content/images/label.png) no-repeat";
    arrow.style.position = "absolute";
    arrow.style.width = "11px";
    arrow.style.height = "10px";
    arrow.style.top = "22px";
    arrow.style.left = "10px";
    arrow.style.overflow = "hidden";
    div.appendChild(arrow);

    div.onmouseover = function() {
        this.style.backgroundColor = "#6BADCA";
        this.style.borderColor = "#0000ff";
        this.style.cursor = "pointer";
        this.getElementsByTagName("span")[0].innerHTML = that._overText;
        arrow.style.backgroundPosition = "0px -20px";
    };

    div.onmouseout = function() {
        this.style.backgroundColor = "#EE5D5B";
        this.style.borderColor = "#BC3B3A";
        this.style.cursor = "hand";
        this.getElementsByTagName("span")[0].innerHTML = that._text;
        arrow.style.backgroundPosition = "0px 0px";
    };

    div.onclick = function () {
        showDetail(that._bianhao);        
    };

    map.getPanes().labelPane.appendChild(div);

    return div;
};
ComplexCustomOverlay.prototype.draw = function() {
    var map = this._map;
    var pixel = map.pointToOverlayPixel(this._point);
    this._div.style.left = pixel.x - parseInt(this._arrow.style.left) + "px";
    this._div.style.top = pixel.y - 40 + "px";
};
ComplexCustomOverlay.prototype.hightlight = function() {
    this._div.style.backgroundColor = "#6BADCA";
    this._div.style.borderColor = "#0000ff";
    this._div.style.cursor = "pointer";
    this._div.style.zIndex = zindex++;
};
ComplexCustomOverlay.prototype.normalshow = function () {
    this._div.style.backgroundColor = "#EE5D5B";
    this._div.style.borderColor = "#BC3B3A";
    this._div.style.cursor = "pointer";
};
ComplexCustomOverlay.prototype.markPoint = function() {
    return this._point;
};
ComplexCustomOverlay.prototype.dispose = function() {
    this._div = null;
};