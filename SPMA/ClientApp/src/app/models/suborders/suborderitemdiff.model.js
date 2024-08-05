"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.SubOrderItemDiff = void 0;
var SubOrderItemDiff = /** @class */ (function () {
    function SubOrderItemDiff() {
        this.number = '';
        this.name = '';
        this.isInProductionCheck = false;
        this.isInProductionOld = false;
        this.isInProductionNew = false;
        this.bookMultiplierCheck = false;
        this.bookMultiplierOld = 0;
        this.bookMultiplierNew = 0;
        this.quantitiesCheck = false;
        this.quantitiesOld = 0;
        this.quantitiesNew = 0;
        this.sourceTypeCheck = false;
        this.sourceTypeOld = 0;
        this.sourceTypeNew = 0;
        this.isAdditionallyPurchasableCheck = false;
        this.isAdditionallyPurchasableOld = false;
        this.isAdditionallyPurchasableNew = false;
        this.wareCodeCheck = false;
        this.wareCodeOld = '';
        this.wareCodeNew = '';
        this.wareLengthCheck = false;
        this.wareLengthOld = 0;
        this.wareLengthNew = 0;
        this.wareUnitCheck = false;
        this.wareUnitOld = '';
        this.wareUnitNew = '';
    }
    return SubOrderItemDiff;
}());
exports.SubOrderItemDiff = SubOrderItemDiff;
//# sourceMappingURL=suborderitemdiff.model.js.map