const { expect } = require ('chai');
const { sum } = require('../functionsCommonJS.js');

describe("Sum function tests", function () {
    it("Sum single number", function () {
        expect(sum([1])).to.equal(1);
    });
});

