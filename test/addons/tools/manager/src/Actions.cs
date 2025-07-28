namespace Godot;

/// <summary>
/// 自定义输入事件
/// </summary>
[InputMap(nameof(InputAction))]
public static partial class Actions
{
    /// <summary>
    /// 以下为godot 内置输入事件
    /// https://gist.github.com/qwe321qwe321qwe321/bbf4b135c49372746e45246b364378c4
    /// </summary>
    public static readonly InputAction UIAccept = new("ui_accept");
    public static readonly InputAction UISelect = new("ui_select");
    public static readonly InputAction UICancel = new("ui_cancel");
    public static readonly InputAction UIFocusNext = new("ui_focus_next");
    public static readonly InputAction UIFocusPrev = new("ui_focus_prev");
    public static readonly InputAction UILeft = new("ui_left");
    public static readonly InputAction UIRight = new("ui_right");
    public static readonly InputAction UIUp = new("ui_up");
    public static readonly InputAction UIDown = new("ui_down");
    public static readonly InputAction UIPageUp = new("ui_page_up");
    public static readonly InputAction UIPageDown = new("ui_page_down");
    public static readonly InputAction UIHome = new("ui_home");
    public static readonly InputAction UIEnd = new("ui_end");
    public static readonly InputAction UICut = new("ui_cut");
    public static readonly InputAction UICopy = new("ui_copy");
    public static readonly InputAction UIPaste = new("ui_paste");
    public static readonly InputAction UIUndo = new("ui_undo");
    public static readonly InputAction UIRedo = new("ui_redo");
    public static readonly InputAction UITextCompletionQuery = new("ui_text_completion_query");
    public static readonly InputAction UITextNewline = new("ui_text_newline");
    public static readonly InputAction UITextNewlineBlank = new("ui_text_newline_blank");
    public static readonly InputAction UITextNewlineAbove = new("ui_text_newline_above");
    public static readonly InputAction UITextIndent = new("ui_text_indent");
    public static readonly InputAction UITextDedent = new("ui_text_dedent");
    public static readonly InputAction UITextBackspace = new("ui_text_backspace");
    public static readonly InputAction UITextBackspaceWord = new("ui_text_backspace_word");
    public static readonly InputAction UITextBackspaceWordMacos = new("ui_text_backspace_word.macos");
    public static readonly InputAction UITextBackspaceAllToLeft = new("ui_text_backspace_all_to_left");
    public static readonly InputAction UITextBackspaceAllToLeftMacos = new("ui_text_backspace_all_to_left.macos");
    public static readonly InputAction UITextDelete = new("ui_text_delete");
    public static readonly InputAction UITextDeleteWord = new("ui_text_delete_word");
    public static readonly InputAction UITextDeleteWordMacos = new("ui_text_delete_word.macos");
    public static readonly InputAction UITextDeleteAllToRight = new("ui_text_delete_all_to_right");
    public static readonly InputAction UITextDeleteAllToRightMacos = new("ui_text_delete_all_to_right.macos");
    public static readonly InputAction UITextCaretLeft = new("ui_text_caret_left");
    public static readonly InputAction UITextCaretWordLeft = new("ui_text_caret_word_left");
    public static readonly InputAction UITextCaretWordLeftMacos = new("ui_text_caret_word_left.macos");
    public static readonly InputAction UITextCaretRight = new("ui_text_caret_right");
    public static readonly InputAction UITextCaretWordRight = new("ui_text_caret_word_right");
    public static readonly InputAction UITextCaretWordRightMacos = new("ui_text_caret_word_right.macos");
    public static readonly InputAction UITextCaretUp = new("ui_text_caret_up");
    public static readonly InputAction UITextCaretDown = new("ui_text_caret_down");
    public static readonly InputAction UITextCaretLineStart = new("ui_text_caret_line_start");
    public static readonly InputAction UITextCaretLineStartMacos = new("ui_text_caret_line_start.macos");
    public static readonly InputAction UITextCaretLineEnd = new("ui_text_caret_line_end");
    public static readonly InputAction UITextCaretLineEndMacos = new("ui_text_caret_line_end.macos");
    public static readonly InputAction UITextCaretPageUp = new("ui_text_caret_page_up");
    public static readonly InputAction UITextCaretPageDown = new("ui_text_caret_page_down");
    public static readonly InputAction UITextCaretDocumentStart = new("ui_text_caret_document_start");
    public static readonly InputAction UITextCaretDocumentStartMacos = new("ui_text_caret_document_start.macos");
    public static readonly InputAction UITextCaretDocumentEnd = new("ui_text_caret_document_end");
    public static readonly InputAction UITextCaretDocumentEndMacos = new("ui_text_caret_document_end.macos");
    public static readonly InputAction UITextCaretAddBelow = new("ui_text_caret_add_below");
    public static readonly InputAction UITextCaretAddBelowMacos = new("ui_text_caret_add_below.macos");
    public static readonly InputAction UITextCaretAddAbove = new("ui_text_caret_add_above");
    public static readonly InputAction UITextCaretAddAboveMacos = new("ui_text_caret_add_above.macos");
    public static readonly InputAction UITextScrollUp = new("ui_text_scroll_up");
    public static readonly InputAction UITextScrollUpMacos = new("ui_text_scroll_up.macos");
    public static readonly InputAction UITextScrollDown = new("ui_text_scroll_down");
    public static readonly InputAction UITextScrollDownMacos = new("ui_text_scroll_down.macos");
    public static readonly InputAction UITextSelectAll = new("ui_text_select_all");
    public static readonly InputAction UITextSelectWordUnderCaret = new("ui_text_select_word_under_caret");
    public static readonly InputAction UITextAddSelectionForNextOccurrence = new("ui_text_add_selection_for_next_occurrence");
    public static readonly InputAction UITextSkipSelectionForNextOccurrence = new("ui_text_skip_selection_for_next_occurrence");
    public static readonly InputAction UITextClearCaretsAndSelection = new("ui_text_clear_carets_and_selection");
    public static readonly InputAction UITextToggleInsertMode = new("ui_text_toggle_insert_mode");
    public static readonly InputAction UITextSubmit = new("ui_text_submit");
    public static readonly InputAction UIGraphDuplicate = new("ui_graph_duplicate");
    public static readonly InputAction UIGraphDelete = new("ui_graph_delete");
    public static readonly InputAction UIFiledialogUpOneLevel = new("ui_filedialog_up_one_level");
    public static readonly InputAction UIFiledialogRefresh = new("ui_filedialog_refresh");
    public static readonly InputAction UIFiledialogShowHidden = new("ui_filedialog_show_hidden");
    public static readonly InputAction UISwapInputDirection = new("ui_swap_input_direction");
}