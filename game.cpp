#include "definitions.hpp"

// Pull definitions
extern Point DEFAULTSWINGTARGET;

GameObject Player;

void update();
void draw(ALLEGRO_FONT *Font);

int main()
{
    if (!al_init()) {
        std::cerr << "Failed to initialize Allegro.\n";
        return -1;
    }

    al_install_keyboard();

    ALLEGRO_TIMER* Timer = al_create_timer(1.0 / 30.0);
    ALLEGRO_EVENT_QUEUE* Queue = al_create_event_queue();
    ALLEGRO_DISPLAY* Disp = al_create_display(1280, 800);
    ALLEGRO_FONT* Font = al_create_builtin_font();

    if (!Timer || !Queue || !Disp || !Font) {
        std::cerr << "Failed to create Allegro objects.\n";
        return -1;
    }

    al_register_event_source(Queue, al_get_keyboard_event_source());
    al_register_event_source(Queue, al_get_display_event_source(Disp));
    al_register_event_source(Queue, al_get_timer_event_source(Timer));

    bool Redraw = true;
    bool Running = true;
    ALLEGRO_EVENT Event;

    al_start_timer(Timer);
    while (Running)
    {
        al_wait_for_event(Queue, &Event);

        if (Event.type == ALLEGRO_EVENT_TIMER) {
            Update();
            Redraw = true;
        } else if (Event.type == ALLEGRO_EVENT_MOUSE_BUTTON_DOWN) {
            cout << "swing" << endl;
        }
        else if (Event.type == ALLEGRO_EVENT_KEY_DOWN || Event.type == ALLEGRO_EVENT_DISPLAY_CLOSE) {
            Running = false;
        }

        if (Redraw && al_is_event_queue_empty(Queue))
        {
            Draw(Font);
            Redraw = false;
        }
    }

    al_destroy_font(Font);
    al_destroy_display(Disp);
    al_destroy_timer(Timer);
    al_destroy_event_queue(Queue);

    return 0;
}

void Update()
{
    Player.ApplyPhysics();
}

void Draw(ALLEGRO_FONT *Font)
{
    al_clear_to_color(al_map_rgb(0, 0, 0));

    // Draw player
    al_draw_filled_rounded_rectangle(Player.Pos.X, Player.Pos.Y,
                                     Player.Pos.X + 50, Player.Pos.Y + 50,
                                     5, 5, al_map_rgb(255, 255, 255));

    // Draw grapple target
    al_draw_filled_rounded_rectangle(DEFAULTSWINGTARGET.X - 25, DEFAULTSWINGTARGET.Y,
                                     DEFAULTSWINGTARGET.X + 25, DEFAULTSWINGTARGET.Y + 50,
                                     5, 5, al_map_rgb(191, 63, 82));
    al_flip_display();
}
