import {expect} from "chai";
import {createCalculator} from "../4.AddSubstract.js";

describe("create calculator function tests", function(){
    it("should return correct output with substraction", function(){
        //arrange
        let calculator = createCalculator(); 

        //act
        calculator.add(10);
        calculator.subtract(5);

        //assert
        expect(calculator.get()).to.equal(5);
    })

    it("should return correct output with addition", function(){
        //arrange
        let calculator = createCalculator(); 

        //act
        calculator.add(10);
        calculator.add(15);

        //assert
        expect(calculator.get()).to.equal(25);
    })

    it("should return correct output with negative numbers", function(){
        //arrange
        let calculator = createCalculator(); 

        //act
        calculator.add(-10);
        calculator.subtract(-5);

        //assert
        expect(calculator.get()).to.equal(-5);
    })

    it("should return correct output with substracting first", function(){
        //arrange
        let calculator = createCalculator(); 

        //act
        calculator.subtract(10);
        calculator.add(20);

        //assert
        expect(calculator.get()).to.equal(10);
    })

    it("should return correct output with string numbers", function(){
        //arrange
        let calculator = createCalculator(); 

        //act
        calculator.subtract('10');
        calculator.add('20');

        //assert
        expect(calculator.get()).to.equal(10);
    })

    it("should return undefined with chars as input", function(){
        //arrange
        let calculator = createCalculator(); 

        //act
        calculator.subtract('$');
        calculator.add('%');

        //assert
        expect(calculator.get()).to.be.NaN;
    })
})