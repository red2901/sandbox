
;;; set some colours etc ...
;;; this highlights the parenthese
(show-paren-mode 1)

;;; this set's the cursor color to red
(set-cursor-color "White")

;;; Hightlight the current region 
(transient-mark-mode t) 
;;; display the time
(display-time)

;;; always use font-lock-mode
(setq global-font-lock-mode t)
(setq font-lock-maximum-decoration t)
;; don't warn on symbolic links
(setq vc-follow-symlinks nil)

;; set the face
(set-foreground-color  "White" ) 
(set-background-color  "Black" )
(set-face-foreground 'region "Green" )
(set-face-background 'region "ForestGreen" )
;; (set-face-foreground 'modeline "Green" )
;; (set-face-background 'modeline "DarkGreen" ) 
(set-background-color  "Black" )
(set-cursor-color  "White" )

;; (set-default-font "-Adobe-Courier-Medium-R-Normal--18-180-75-75-M-110-ISO8859-1")

;;
;; spaces, not tabs ! 
(setq-default indent-tabs-mode nil)

;; turn off blinking cursor mode
(if (fboundp 'blink-cursor-mode) (blink-cursor-mode 0))

;; turn the font lock modes on by default 
;; hooks to major modes
(add-hook 'dired-mode-hook 'turn-on-font-lock) 
(add-hook 'emacs-lisp-mode-hook 'turn-on-font-lock)

;;
;; setting up some major modes
;;
;; i think sql mode is already loaded so I'm going to list some of it's files
(load "sql")
(autoload 'sql-mode "sql") 
(add-hook 'sql-mode-hook 'turn-on-font-lock)
(setq auto-mode-alist (cons '("\.ddl$" . sql-mode) auto-mode-alist))
(setq auto-mode-alist (cons '("\.sp$" . sql-mode) auto-mode-alist))
(setq auto-mode-alist (cons '("\.dml$" . sql-mode) auto-mode-alist)) 
(setq auto-mode-alist (cons '("\.sql$" . sql-mode) auto-mode-alist))

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; js2-mode
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(add-to-list 'load-path "~/emacs-lisp/js2-mode")
(autoload 'js2-mode "js2-mode") 
(add-to-list 'auto-mode-alist '("\\.js\\'" . js2-mode))
(add-to-list 'interpreter-mode-alist '("nodejs" . js2-mode))

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; ace jump mode
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(add-to-list 'load-path "~/emacs-lisp/ace-jump-mode")
(autoload
  'ace-jump-mode
  "ace-jump-mode"
  "Emacs quick move minor mode"
  t)
;; you can select the key you prefer to
(define-key global-map (kbd "C-c SPC") 'ace-jump-mode)

;; 
;; enable a more powerful jump back function from ace jump mode
;;
(autoload
  'ace-jump-mode-pop-mark
  "ace-jump-mode"
  "Ace jump back:-)"
  t)
(eval-after-load "ace-jump-mode"
  '(ace-jump-mode-enable-mark-sync))
(define-key global-map (kbd "C-x SPC") 'ace-jump-mode-pop-mark)

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; autocomplete setup
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; this is popup
(add-to-list 'load-path "~/emacs-lisp/popup")
(require 'popup)

;; this is auto-complete
(add-to-list 'load-path "~/.emacs.d")
(require 'auto-complete-config)
(ac-config-default)

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;; this is c-mode
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
(add-hook 'c-mode-hook '(lambda () (c-set-style "ellemtel")))
(add-hook 'c++-mode-hook '(lambda () (c-set-style "ellemtel"))) 
(setq auto-mode-alist (cons '("\.cpp$" . c++-mode) auto-mode-alist))
(setq auto-mode-alist (cons '("\.hpp$" . c++-mode) auto-mode-alist))

(add-hook 'c++-mode-hook 'turn-on-font-lock)
(add-hook 'c-mode-hook 'turn-on-font-lock) 

;; 
;; this is csharp-mode
(autoload 'csharp-mode "csharp-mode" "Major mode for editing C# code." t)
(setq auto-mode-alist
   (append '(("\\.cs$" . csharp-mode)) auto-mode-alist))

;;
;; this is python-mode
(setq auto-mode-alist (cons '("\.pyw$" . python-mode) auto-mode-alist)) 
(setq auto-mode-alist (cons '("\.py$" . python-mode) auto-mode-alist))


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; Golang setup
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;; http://tleyden.github.io/blog/2014/05/22/configure-emacs-as-a-go-editor-from-scratch/

;;
;; this is go-mode
;; emacs library path
(add-to-list 'load-path "~/emacs-lisp/go-mode")
(require 'go-mode)

;; update emacs config for godoc
;; fix the PATH variable
(defun set-exec-path-from-shell-PATH ()
  (let ((path-from-shell (shell-command-to-string "$SHELL -i -c 'echo $PATH'")))
    (setenv "PATH" path-from-shell)
    (setq exec-path (split-string path-from-shell path-separator))))

(if window-system (set-exec-path-from-shell-PATH))
;; do this only if the above doesn't work
;; (setenv "PATH" "")
;; (setenv "GOPATH" "")

;; automatically call gofmt on save
(setq exec-path (cons "/usr/local/go/bin" exec-path))
(add-to-list 'exec-path "/home/kin/dev/src/gows/bin")
;; (add-hook 'before-save-hook 'gofmt-before-save)

; Go Oracle
(load-file "$GOPATH/src/code.google.com/p/go.tools/cmd/oracle/oracle.el")

;; godef, go build (M-x compile)
(defun my-go-mode-hook ()
  ; Use goimports instead of go-fmt
  (setq gofmt-command "goimports")
  ; Call Gofmt before saving                                                    
  (add-hook 'before-save-hook 'gofmt-before-save)
  ; Customize compile command to run go build
  (if (not (string-match "go" compile-command))
      (set (make-local-variable 'compile-command)
           "go build -v && go test -v && go vet"))
  ; Godef jump key binding                                                      
  (local-set-key (kbd "M-.") 'godef-jump))
  ; Go Oracle
  (go-oracle-mode)
(add-hook 'go-mode-hook 'my-go-mode-hook)

;; go aware autocomplete
(add-to-list 'load-path "~/emacs-lisp/go-autocomplete")
(require 'go-autocomplete)
(require 'auto-complete-config)

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; global set keys
;; C-x C-y for shell
(global-set-key "\C-x\C-y" 'shell)
(global-set-key "\C-x\C-r" 'replace-string)
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(add-to-list 'load-path "~/emacs-lisp/markdown")
(autoload 'markdown-mode "markdown-mode" "Major mode for editing Markdown files" t)
(autoload 'gfm-mode "gfm-mode" "Major mode for editing GitHub Flavored Markdown files." t)
(add-to-list 'auto-mode-alist '("\\.text\\'" . markdown-mode))
(add-to-list 'auto-mode-alist '("\\.markdown\\'" . markdown-mode))
(add-to-list 'auto-mode-alist '("\\.md\\'" . markdown-mode))
(add-to-list 'auto-mode-alist '("README\\.md\\'" . gfm-mode))

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; yasnippet
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

(add-to-list 'load-path "~/emacs-lisp/yasnippet")
(require 'yasnippet)
(yas-global-mode 1)
