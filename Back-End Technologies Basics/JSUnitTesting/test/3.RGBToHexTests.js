import { expect } from "chai";
import { rgbToHexColor } from '../3.RGBToHex.js';

describe("RGB to Hex function tests", function () {
    it("should return correct Hex for red color ", function () {
        //arrange
        let red = 255;
        let green = 0;
        let blue = 0;
        let result;

        //act
        result = rgbToHexColor(red, green, blue);

        //act
        expect(result).to.equal('#FF0000')
    })

    it("should return correct Hex for green color", function () {
        //act
        let result = rgbToHexColor(0, 128, 0)

        //assert
        expect(result).to.equal('#008000')
    })

    it("should return correct Hex for blue color", function () {
        //act
        let result = rgbToHexColor(0, 0, 255)

        //assert
        expect(result).to.equal('#0000FF')
    })

    it("should return undefined with non-number input", function () {
        //act
        let result = rgbToHexColor('a', 'b', 'c')

        //assert
        expect(result).to.be.undefined;
    })

    it("should return undefined with above range numbers", function () {
        //act
        let result = rgbToHexColor(-20, 0, 300)

        //assert
        expect(result).to.be.undefined;
    })

    it("should return correct Hex with random color", function () {
        //act
        let result = rgbToHexColor('$', 5, 255)

        //assert
        expect(result).to.be.undefined;
    })

    it("should return correct Hex with random color", function () {
        //act
        let result = rgbToHexColor(255)

        //assert
        expect(rgbToHexColor(255)).to.be.undefined;
    })

    it("should return correct Hex with lower boundary", function () {
        //act
        let result = rgbToHexColor(0, 0, 0)

        //assert
        expect(result).to.equal('#000000');
    })

    it("should return correct Hex with upper boundary", function () {
        //act
        let result = rgbToHexColor(255, 255, 255)

        //assert
        expect(result).to.equal('#FFFFFF');
    })

    it("should return undefined with negative number", function () {
        //act
        let result = rgbToHexColor(-1, 255, 255)

        //assert
        expect(result).to.be.undefined;
    })

    it("should return undefined for bigger than 255 number", function () {
        //act
        let result = rgbToHexColor(255, 256, 255)

        //assert
        expect(result).to.be.undefined;
    })

    it("should return undefined for decimal", function () {
        //act
        let result = rgbToHexColor(255, 12.5, 255)

        //assert
        expect(result).to.be.undefined;
    })

    it("should return undefined for decimal red color", function () {
        //act
        let result = rgbToHexColor(12.5, 122, 255)

        //assert
        expect(result).to.be.undefined;
    })

    it("should return undefined for decimal green color", function () {
        //act
        let result = rgbToHexColor(255, 12.5, 255)
        
        //assert
        expect(result).to.be.undefined;
    })

    it("should return undefined for decimal blue color", function () {
        //act
        let result = rgbToHexColor(255, 145, 12.5)

        //assert
        expect(result).to.be.undefined;
    })
})
