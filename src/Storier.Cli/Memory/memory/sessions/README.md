# Podsumowanie folderu Memory/memory/sessions

Cel:
- Ten folder przechowuje sesje gracza i pliki pomocnicze dla dalszego przetwarzania przez AI.

Struktura:
- 1/ — zakończona sesja gracza. Zawiera summary.md z podsumowaniem przebiegu i kluczowymi faktami.
- 2/ — materiał startowy do kontynuacji (rozpoczęcia) sesji 2. Zawiera start.md z instrukcjami i stanem początkowym.

Jak AI ma używać tych plików:
- Wczytać summary.md (folder 1) jako kontekst historyczny — to źródło faktów, decyzji i wyników poprzedniej sesji.
- Wczytać start.md (folder 2) jako instrukcję inicjującą nową sesję — zawiera cele, punkt wyjścia i ograniczenia.
- Zachować spójność z faktami z summary.md i respektować ograniczenia wymienione w start.md.
- Generować dalsze akcje/rozmowy jako kontynuację stanu z obu plików; na początku podać krótkie streszczenie kontekstu (1–2 zdania), potem proponować kroki/odpowiedzi.

Uwagi:
- Pliki odnoszą się do ścieżek względnych: 1/summary.md i 2/start.md.
- Zachować formatowanie i nie modyfikować historycznych danych w summary.md — używać jako źródła prawdy.
- Jeśli start.md zawiera otwarte pytania lub brakujące dane, AI powinna najpierw zaproponować pytania uzupełniające.
- Przy generowaniu nowych plików/zmian zapisać je w osobnych plikach, nie nadpisywać summary.md bez wersjonowania.