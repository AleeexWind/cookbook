Feature: Menu plan
    As an authorized user I want to plan meals for a week
    so that I can drag recipes from a picker into day slots with shared portions

    Scenario: Guest cannot open the menu plan
        Given I am not logged in
        When I open the menu plan page
        Then I should not see the week calendar
        And I should be asked to log in

    Scenario: Authorized user sees recipe picker and empty week calendar
        Given I am logged in as "alice"
        And There are recipes in the database
        When I open the menu plan page
        Then I should see a recipe picker with recipes I can access
        And I should see a week calendar with slots for Monday to Sunday
        And Each day should have breakfast, lunch and dinner slots
        And Portions should be set to 2 by default

    Scenario: Drag a recipe from the picker into a day slot
        Given I am logged in as "alice"
        And There is a public recipe "Pasta Carbonara" for 2 portions
        When I open the menu plan page
        And I drag "Pasta Carbonara" from the recipe picker to Monday dinner
        Then Monday dinner should contain "Pasta Carbonara"

    Scenario: Plan portions scale recipe ingredients
        Given I am logged in as "alice"
        And There is a public recipe "Pasta Carbonara" for 2 portions with ingredient "Spaghetti" quantity 200 g
        And I have placed "Pasta Carbonara" in Monday dinner
        When I open the menu plan page
        And I set plan portions to 3
        Then The plan portions should be 3
        And Ingredient "Spaghetti" for "Pasta Carbonara" in the plan should be 300 g
