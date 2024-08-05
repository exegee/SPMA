"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ProductionState = void 0;
var ProductionState;
(function (ProductionState) {
    ProductionState[ProductionState["Idle"] = 1] = "Idle";
    ProductionState[ProductionState["Awaiting"] = 2] = "Awaiting";
    ProductionState[ProductionState["inCut"] = 3] = "inCut";
    ProductionState[ProductionState["alreadyCut"] = 4] = "alreadyCut";
    ProductionState[ProductionState["PurchaseItem"] = 100] = "PurchaseItem";
})(ProductionState = exports.ProductionState || (exports.ProductionState = {}));
//# sourceMappingURL=productionstate.js.map