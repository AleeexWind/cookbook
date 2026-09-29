Feature: Get recipe ingredients
    As a user I want to see recipe ingredients with quantities and units
    so that I know what I need to cook

    Scenario: View ingredients for a selected recipe
        Given There is a public recipe "Pasta Carbonara" for 2 portions in the database
        And The recipe has the following ingredients
        When I open the recipe "Pasta Carbonara" from the recipes page
        Then I should see ingredients with name, quantity, unit and portions set to 2

    Examples:
       | Ingredient | Quantity | Unit  |
       | Spaghetti  | 200      | g     |
       | Eggs       | 2        | units |
       | Bacon      | 100      | g     |
       | Milk       | 50       | ml    |

    Scenario: Scale ingredients when portions change
        Given There is a public recipe "Pasta Carbonara" for 2 portions in the database
        And The recipe has the following ingredients for 2 portions
        When I open the recipe "Pasta Carbonara" from the recipes page
        And I set portions to 4
        Then I should see ingredient quantities scaled for 4 portions

    Examples:
       | Ingredient | Quantity for 2 portions | Quantity for 4 portions | Unit  |
       | Spaghetti  | 200                     | 400                     | g     |
       | Eggs       | 2                       | 4                       | units |
       | Bacon      | 100                     | 200                     | g     |
       | Milk       | 50                      | 100                     | ml    |
