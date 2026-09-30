Feature: Shopping list
    As an authorized user I want to generate a shopping list from my menu plan
    so that ingredients are aggregated for the planned meals

    Scenario: Guest cannot generate a shopping list
        Given I am not logged in
        When I open the menu plan page
        Then I should not be able to generate a shopping list

    Scenario: Cannot generate shopping list from an empty plan
        Given I am logged in as "alice"
        And My menu plan has no recipes
        When I open the menu plan page
        Then I should not be able to generate a shopping list

    Scenario: Generate shopping list aggregates ingredients with plan portions
        Given I am logged in as "alice"
        And There is a public recipe "Pasta Carbonara" for 2 portions with ingredients
        And There is a public recipe "Chicken Stir Fry" for 2 portions with ingredients
        And My menu plan portions are 3
        And I have placed "Pasta Carbonara" in Monday dinner
        And I have placed "Chicken Stir Fry" in Tuesday lunch
        When I open the menu plan page
        And I generate the shopping list
        Then I should see aggregated ingredients scaled to 3 portions

    Examples:
       | Ingredient | Quantity | Unit  |
       | Spaghetti  | 300      | g     |
       | Eggs       | 3        | units |
       | Bacon      | 150      | g     |
       | Milk       | 75       | ml    |
       | Chicken    | 450      | g     |
       | Rice       | 300      | g     |
       | Soy sauce  | 45       | ml    |

    Scenario: Duplicate recipes in the plan sum ingredient quantities
        Given I am logged in as "alice"
        And There is a public recipe "Pasta Carbonara" for 2 portions with ingredient "Spaghetti" quantity 200 g
        And My menu plan portions are 2
        And I have placed "Pasta Carbonara" in Monday dinner
        And I have placed "Pasta Carbonara" in Wednesday breakfast
        When I open the menu plan page
        And I generate the shopping list
        Then Ingredient "Spaghetti" should be 400 g

    Scenario: Shopping list updates when a recipe is removed from the plan
        Given I am logged in as "alice"
        And There is a public recipe "Pasta Carbonara" for 2 portions with ingredient "Spaghetti" quantity 200 g
        And There is a public recipe "Chicken Stir Fry" for 2 portions with ingredient "Chicken" quantity 300 g
        And My menu plan portions are 2
        And I have placed "Pasta Carbonara" in Monday dinner
        And I have placed "Chicken Stir Fry" in Tuesday lunch
        And I have generated the shopping list
        When I remove "Pasta Carbonara" from Monday dinner
        Then The shopping list should no longer include "Spaghetti"
        And Ingredient "Chicken" should be 300 g

    Scenario: Export shopping list to CSV
        Given I am logged in as "alice"
        And There is a public recipe "Pasta Carbonara" for 2 portions with ingredient "Spaghetti" quantity 200 g
        And My menu plan portions are 2
        And I have placed "Pasta Carbonara" in Monday dinner
        And I have generated the shopping list
        When I export the shopping list to CSV
        Then I should receive a CSV file with columns Ingredient, Quantity and Unit
        And The CSV should contain a row for "Spaghetti" with quantity 200 and unit "g"

    Scenario: Cannot export shopping list when it was not generated
        Given I am logged in as "alice"
        And My menu plan has no recipes
        When I open the menu plan page
        Then I should not be able to export the shopping list to CSV
